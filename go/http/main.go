package main

import (
	"context"
	"io/ioutil"
	"log"
	"os"

	"contrib.go.opencensus.io/exporter/stackdriver"
	"go.opencensus.io/stats/view"
	"go.opencensus.io/trace"

	"cloud.google.com/go/storage"
)

func downloadImage() ([]byte, error) {
	ctx := context.Background()
	client, err := storage.NewClient(ctx)

	if err != nil {
		log.Fatalf("Failed to create client: %v", err)
	}
	bucket := os.Getenv("GOOGLE_CLOUD_STORAGE_BUCKET")

	rc, err := client.Bucket(bucket).Object("demo-image.jpg").NewReader(ctx)
	if err != nil {
		return nil, err
	}
	defer rc.Close()

	data, err := ioutil.ReadAll(rc)
	if err != nil {
		return nil, err
	}
	return data, nil
}

func main() {

	exporter, erra := stackdriver.NewExporter(stackdriver.Options{ProjectID: os.Getenv("GOOGLE_CLOUD_PROJECT")})
	if erra != nil {
		log.Fatal(erra)
	}
	view.RegisterExporter(exporter)
	trace.RegisterExporter(exporter)
	trace.ApplyConfig(trace.Config{DefaultSampler: trace.AlwaysSample()})

	for i := 0; i < 10; i++ {
		downloadImage()
	}

	exporter.Flush()
}
