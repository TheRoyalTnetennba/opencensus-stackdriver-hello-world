package main

import (
	"context"
	"go.opencensus.io/stats"
	"math/rand"
	"time"
)

func longOp(r *rand.Rand) {
	t := time.Duration(1000 * 1000 * r.Intn(500))
	time.Sleep(t)
}

func main() {
	ctx := context.Background()
	s := rand.NewSource(time.Now().UnixNano())
	r := rand.New(s)

	startTime := time.Now()
	for i := 0; i < 10; i++ {
		longOp(r)
	}
	elapsedSeconds := time.Since(startTime).Seconds()
	measurement := stats.Float64("TimeElapsed", "Time in seconds to run longOp 10 times", "seconds")
	stats.Record(ctx, measurement.M(elapsedSeconds))
}
