import io
import os

# Imports the Google Cloud client library
from google.cloud import vision
from google.cloud.vision import types

from opencensus.trace.exporters import stackdriver_exporter
from opencensus.trace import tracer as tracer_module
from opencensus.trace.ext.google_cloud_clientlibs.trace import trace_integration


def get_image(path):
    file_name = os.path.join(os.path.dirname(__file__), path)
    with io.open(file_name, "rb") as image_file:
        content = image_file.read()
    return types.Image(content=content)


def get_tracer():
    exporter = stackdriver_exporter.StackdriverExporter(
        project_id=os.environ["GOOGLE_CLOUD_PROJECT"]
    )
    return tracer_module.Tracer(exporter=exporter)


def main(repetitions=10):
    tracer = get_tracer()
    trace_integration(tracer)
    image = get_image("../../resources/demo-image.jpg")
    client = vision.ImageAnnotatorClient()

    for _ in range(repetitions):
        labels = client.label_detection(image=image).label_annotations

    for label in labels:
        print(label.description)


if __name__ == "__main__":
    main()
