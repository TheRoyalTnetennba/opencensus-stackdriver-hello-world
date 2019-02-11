package com.helloworld;

import io.opencensus.common.Scope;
import io.opencensus.trace.Tracer;
import io.opencensus.trace.Tracing;
import io.opencensus.trace.config.TraceParams;
import io.opencensus.trace.samplers.Samplers;
import io.opencensus.exporter.trace.stackdriver.StackdriverTraceConfiguration;
import io.opencensus.exporter.trace.stackdriver.StackdriverTraceExporter;
import com.google.cloud.storage.Bucket;
import com.google.cloud.storage.BucketInfo;
import com.google.cloud.storage.Storage;
import com.google.cloud.storage.StorageOptions;
import com.google.cloud.storage.Blob;
import com.google.cloud.storage.BlobId;

import com.google.errorprone.annotations.Var;
import com.google.protobuf.ByteString;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import java.io.IOException;

public class HelloWorld {
    private static final Tracer tracer = Tracing.getTracer();

    private static void doHelloWorld() {
        try {
            String projectId = requiredProperty("GOOGLE_CLOUD_PROJECT");

            StackdriverTraceExporter
                    .createAndRegister(StackdriverTraceConfiguration.builder().setProjectId(projectId).build());

            Storage storage = StorageOptions.getDefaultInstance().getService();

            String bucketName = requiredProperty("GOOGLE_CLOUD_STORAGE_BUCKET");  // "my-new-bucket";
        
            Blob blob = storage.get(BlobId.of(bucketName, "demo-image.jpg"));

            try (Scope ss =
                tracer
                    .spanBuilder("JavaHTTP")
                    .setRecordEvents(true)
                    .setSampler(Samplers.alwaysSample())
                    .startScopedSpan()) {
                for (int i = 0; i < 10; i++) {
                    blob.downloadTo(Paths.get("../../downloads/demo-image.jpg"));
                }
                tracer.getCurrentSpan().addAnnotation("Finished initial work");
            }

        } catch (IOException e) {
            System.err.println("Exception while running HelloWorld: " + e.getMessage());
            e.printStackTrace();
            System.exit(1);
        }

        System.exit(0);
    }

    public static void main(String[] args) throws IOException, InterruptedException {
        Tracing.getTraceConfig().updateActiveTraceParams(
                TraceParams.DEFAULT.toBuilder().setSampler(Samplers.probabilitySampler(1)).build());

        doHelloWorld();
    }

    private static String requiredProperty(String prop) {
        String value = System.getenv(prop);
        if (value == null) {
            throw new IllegalArgumentException("Missing required environment variable: " + prop);
        }
        return value;
    }
}

