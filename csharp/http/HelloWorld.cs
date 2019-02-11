namespace HelloWorld
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading;
    using OpenCensus.Exporter.Stackdriver;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;
    using OpenCensus.Stats.Measures;
    using OpenCensus.Tags;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Sampler;
    using Google.Cloud.Storage.V1;

    public class QuickStart
    {
        public static void Main(string[] args)
        {
            var exporter = new StackdriverExporter(
                Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT"),
                Tracing.ExportComponent,
                Stats.ViewManager);

            var spanBuilder = Tracing.Tracer
                .SpanBuilder("incoming request")
                .SetRecordEvents(true)
                .SetSampler(Samplers.AlwaysSample);
            exporter.Start();


            StorageClient storageClient = StorageClient.Create();
            var storage = StorageClient.Create();
            var objectName = "demo-image.jpg";
            var localPath = "../../downloads/demo-image.jpg";
            var bucketName = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_STORAGE_BUCKET");


            using (var scopedSpan = spanBuilder.StartScopedSpan())
            {
                for (var i = 0; i < 10; i++)
                {
                    using (var outputFile = File.OpenWrite(localPath))
                    {
                        storage.DownloadObject(bucketName, objectName, outputFile);
                    }
                }

            }

            Thread.Sleep(TimeSpan.FromMilliseconds(5000));

        }
    }
}
