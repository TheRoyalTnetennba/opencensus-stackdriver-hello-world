// Imports the Google Cloud client library
const {Storage} = require('@google-cloud/storage');


// Creates a client
const storage = new Storage();

const tracing = require('@opencensus/nodejs');
const stackdriver = require('@opencensus/exporter-stackdriver');
const propagation = require('@opencensus/propagation-stackdriver');

const sd = propagation.v1;
const exporter = new stackdriver.StackdriverTraceExporter({ projectId: process.env.GOOGLE_CLOUD_PROJECT });

tracing.start({ propagation: sd, exporter: exporter, samplingRate: 1 });

const options = {
    destination: '../../downloads/demo-image.jpg',
  };

const bucketName = process.env.GOOGLE_CLOUD_STORAGE_BUCKET;
const srcFilename = 'demo-image.jpg';

const downloadImages = (repetition, rootSpan, cb) => {
    storage
        .bucket(bucketName)
        .file(srcFilename)
        .download(options)
        .then(() => {
            if (repetition === 0) {
                rootSpan.end();
            } else {
                cb(repetition - 1, rootSpan, cb);
            }
        });
}

const explicitSpan = () => {
    const rootSpanOptions = { name: 'nodejsHttp' };
    tracing.tracer.startRootSpan(rootSpanOptions, (rootSpan) => {
        downloadImages(9, rootSpan, downloadImages);
    });
}

explicitSpan();
