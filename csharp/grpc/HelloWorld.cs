namespace HelloWorld
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using OpenCensus.Exporter.Stackdriver;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;
    using OpenCensus.Stats.Measures;
    using OpenCensus.Tags;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Sampler;
    using Google.Cloud.Vision.V1;

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


            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile("../../resources/demo-image.jpg");

            using (var scopedSpan = spanBuilder.StartScopedSpan())
            {
                var response = client.DetectLabels(image);
                for (var i = 0; i < 9; i++)
                {
                    response = client.DetectLabels(image);
                }

                foreach (var annotation in response)
                {
                    if (annotation.Description != null)
                        Console.WriteLine(annotation.Description);
                }
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(5000));

        }
    }
}
