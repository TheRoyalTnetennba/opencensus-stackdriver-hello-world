import io
import os

# Imports the Google Cloud client library
from google.cloud import storage

from opencensus.trace.exporters import stackdriver_exporter
from opencensus.trace import tracer as tracer_module
from opencensus.trace.ext.google_cloud_clientlibs.trace import trace_integration


def get_tracer():
    exporter = stackdriver_exporter.StackdriverExporter(
        project_id=os.environ["GOOGLE_CLOUD_PROJECT"]
    )
    return tracer_module.Tracer(exporter=exporter)

def main(repetitions=10):
    tracer = get_tracer()
    trace_integration(tracer)
    storage_client = storage.Client()
    bucket = storage_client.get_bucket(os.environ["GOOGLE_CLOUD_STORAGE_BUCKET"])
    blob = bucket.blob('demo-image.jpg')
    with tracer.span(name='python-http') as span1:
        for _ in range(repetitions):
            blob.download_to_filename('../../downloads/demo-image.jpg')


if __name__ == "__main__":
    main()
