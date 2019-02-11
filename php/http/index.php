<?php
require __DIR__ . '/vendor/autoload.php';

use Google\Cloud\Storage\StorageClient;
use OpenCensus\Trace\Tracer;
use OpenCensus\Trace\Exporter\StackdriverExporter;
use OpenCensus\Trace\Sampler\AlwaysSampleSampler;

Tracer::start(new StackdriverExporter(), [
    'sampler' =>new AlwaysSampleSampler()
]);

function detect_label()
{
    $storage = new StorageClient([
        'projectId' => getenv('GOOGLE_CLOUD_PROJECT')
    ]);

    $bucketName = getenv('GOOGLE_CLOUD_STORAGE_BUCKET');
    $storage = new StorageClient();
    $bucket = $storage->bucket($bucketName);
    $object = $bucket->object('demo-image.jpg');
    $destination = '../../downloads/demo-image.jpg';

    $span = Tracer::startSpan(['name' => 'php-http']);
    $scope = Tracer::withSpan($span);

    for ($x = 0; $x < 10; $x++) {
        $object->downloadToFile($destination);
    }
}

detect_label()

?>
