package main

import (
	"context"
	"fmt"
	"log"
	"os"

	"contrib.go.opencensus.io/exporter/stackdriver"
	"go.opencensus.io/stats/view"
	"go.opencensus.io/trace"

	vision "cloud.google.com/go/vision/apiv1"
)

// findLabels gets labels from the Vision API for an image at the given file path.
func findLabels() ([]string, error) {
	ctx := context.Background()

	// Create the client.
	client, err := vision.NewImageAnnotatorClient(ctx)
	if err != nil {
		return nil, err
	}

	// Open the file.
	f, err := os.Open("../../resources/demo-image.jpg")
	if err != nil {
		return nil, err
	}
	image, err := vision.NewImageFromReader(f)
	if err != nil {
		return nil, err
	}

	// Perform the request.
	annotations, err := client.DetectLabels(ctx, image, nil, 10)
	if err != nil {
		return nil, err
	}

	var labels []string
	for _, annotation := range annotations {
		labels = append(labels, annotation.Description)
	}
	return labels, nil
}

func main() {

	exporter, erra := stackdriver.NewExporter(stackdriver.Options{ProjectID: os.Getenv("GOOGLE_CLOUD_PROJECT")})
	if erra != nil {
		log.Fatal(erra)
	}
	view.RegisterExporter(exporter)
	trace.RegisterExporter(exporter)
	trace.ApplyConfig(trace.Config{DefaultSampler: trace.AlwaysSample()})

	for i := 0; i < 9; i++ {
		findLabels()
	}

	labels, err := findLabels()
	if err != nil {
		fmt.Fprintf(os.Stderr, "%v\n", err)
		os.Exit(1)
	}
	if len(labels) == 0 {
		fmt.Println("No labels found.")
	} else {
		fmt.Println("Found labels:")
		for _, label := range labels {
			fmt.Println(label)
		}
	}

	exporter.Flush()
}
