<?php
require __DIR__ . '/vendor/autoload.php';

use OpenCensus\Trace\Tracer;
use OpenCensus\Trace\Exporter\StackdriverExporter;
use Google\Cloud\Vision\V1\ImageAnnotatorClient;
use OpenCensus\Trace\Sampler\AlwaysSampleSampler;

Tracer::start(new StackdriverExporter(), [
    'sampler' =>new AlwaysSampleSampler()
]);

function detect_label($path)
{
    $imageAnnotator = new Google\Cloud\Vision\V1\ImageAnnotatorClient();

    # annotate the image
    $image = file_get_contents($path);

    $span = Tracer::startSpan(['name' => 'php-vision']);
    $scope = Tracer::withSpan($span);

    for ($x = 0; $x < 9; $x++) {
        $imageAnnotator->labelDetection($image);
    } 

    $response = $imageAnnotator->labelDetection($image);

    $scope->close();

    
    $labels = $response->getLabelAnnotations();

    if ($labels) {
        print("Labels:" . PHP_EOL);
        foreach ($labels as $label) {
            print($label->getDescription() . PHP_EOL);
        }
    } else {
        print('No label found' . PHP_EOL);
    }

    $imageAnnotator->close();
}

detect_label(getenv("IMAGE"))

?>
