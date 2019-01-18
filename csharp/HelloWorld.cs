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
            var image = Image.FromFile("../resources/demo-image.jpg");

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

            Thread.Sleep(TimeSpan.FromMilliseconds(15100));

        }
    }
}

// internal class TestStackdriver
// {
//     private static ITracer tracer = Tracing.Tracer;
//     private static ITagger tagger = Tags.Tagger;

//     private static IStatsRecorder statsRecorder = Stats.StatsRecorder;
//     private static readonly IMeasureDouble VideoSize = MeasureDouble.Create("my_org/measure/video_size2", "size of processed videos", "MiB");
//     private static readonly ITagKey FrontendKey = TagKey.Create("my_org/keys/frontend");

//     private static long MiB = 1 << 20;



//     internal static object Run(string projectId)
//     {


//         ITagContextBuilder tagContextBuilder = tagger.CurrentBuilder.Put(FrontendKey, TagValue.Create("mobile-ios9.3.5"));




//         using (var scopedTags = tagContextBuilder.BuildScoped())
//         {
//             using (var scopedSpan = spanBuilder.StartScopedSpan())
//             {
//                 tracer.CurrentSpan.AddAnnotation("Start processing video.");

//                 Thread.Sleep(TimeSpan.FromMilliseconds(10));
//                 statsRecorder.NewMeasureMap()
//                     .Put(VideoSize, 25 * MiB)
//                     .Record();

//                 tracer.CurrentSpan.AddAnnotation("Finished processing video.");
//             }
//         }

//         Thread.Sleep(TimeSpan.FromMilliseconds(5100));

//         var viewData = Stats.ViewManager.GetView(VideoSizeViewName);

//         Console.WriteLine(viewData);

//         Console.WriteLine("Done... wait for events to arrive to backend!");
//         Console.ReadLine();

//         return null;
//     }
// }
// }