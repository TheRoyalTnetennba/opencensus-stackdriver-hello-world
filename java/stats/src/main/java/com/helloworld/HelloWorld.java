package com.helloworld;

import io.opencensus.trace.Tracing;
import io.opencensus.trace.config.TraceParams;
import io.opencensus.trace.samplers.Samplers;
import io.opencensus.exporter.trace.stackdriver.StackdriverTraceConfiguration;
import io.opencensus.exporter.trace.stackdriver.StackdriverTraceExporter;
import com.google.cloud.vision.v1.AnnotateImageRequest;
import com.google.cloud.vision.v1.AnnotateImageResponse;
import com.google.cloud.vision.v1.BatchAnnotateImagesResponse;
import com.google.cloud.vision.v1.EntityAnnotation;
import com.google.cloud.vision.v1.Feature;
import com.google.cloud.vision.v1.Feature.Type;
import com.google.errorprone.annotations.Var;
import com.google.cloud.vision.v1.Image;
import com.google.cloud.vision.v1.ImageAnnotatorClient;
import com.google.protobuf.ByteString;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import java.io.IOException;

public class HelloWorld {
    private static void doHelloWorld() {
        try (ImageAnnotatorClient vision = ImageAnnotatorClient.create()) {
            // Instantiates a client\
            String projectId = requiredProperty("GOOGLE_CLOUD_PROJECT");

            StackdriverTraceExporter
                    .createAndRegister(StackdriverTraceConfiguration.builder().setProjectId(projectId).build());
            // The path to the image file to annotate
            String fileName = "../../../resources/demo-image.jpg";

            // Reads the image file into memory
            Path path = Paths.get(fileName);
            byte[] data = Files.readAllBytes(path);
            ByteString imgBytes = ByteString.copyFrom(data);

            // Builds the image annotation request
            List<AnnotateImageRequest> requests = new ArrayList<>();
            Image img = Image.newBuilder().setContent(imgBytes).build();
            Feature feat = Feature.newBuilder().setType(Type.LABEL_DETECTION).build();
            AnnotateImageRequest request = AnnotateImageRequest.newBuilder().addFeatures(feat).setImage(img).build();
            requests.add(request);

            // Performs label detection on the image file
            BatchAnnotateImagesResponse response = vision.batchAnnotateImages(requests);
            for (Integer i = 0; i < 9; i++) {
                response = vision.batchAnnotateImages(requests);
            }
            List<AnnotateImageResponse> responses = response.getResponsesList();

            for (AnnotateImageResponse res : responses) {
                if (res.hasError()) {
                    System.out.printf("Error: %s\n", res.getError().getMessage());
                    return;
                }

                for (EntityAnnotation annotation : res.getLabelAnnotationsList()) {
                    System.out.printf("description: %s\n", annotation.getDescription());
                }
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
