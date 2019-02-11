// Imports the Google Cloud client library
const vision = require('@google-cloud/vision');


// Creates a client
const client = new vision.ImageAnnotatorClient();

const tracing = require('@opencensus/nodejs');
const stackdriver = require('@opencensus/exporter-stackdriver');
const propagation = require('@opencensus/propagation-stackdriver');

const sd = propagation.v1;
const exporter = new stackdriver.StackdriverTraceExporter({ projectId: process.env.GOOGLE_CLOUD_PROJECT });

tracing.start({ propagation: sd, exporter: exporter, samplingRate: 1 });

const requestLabels = (repetition, rootSpan, cb) => {
    client
        .labelDetection('../resources/demo-image.jpg')
        .then(results => {
            const labels = results[0].labelAnnotations;
            if (repetition === 0) {
                console.log('Labels:');
                labels.forEach(label => console.log(label.description));
                rootSpan.end();
            } else {
                cb(repetition - 1, rootSpan, cb)
            }
        })
        .catch(err => {
            console.error('ERROR:', err);
        });
}



const explicitSpan = () => {
    const rootSpanOptions = { name: 'nodejs-vision' };
    tracing.tracer.startRootSpan(rootSpanOptions, (rootSpan) => {
        requestLabels(9, rootSpan, requestLabels);
    });
}

const implicitSpan = () => {
    requestLabels(9, { end: () => {} }, requestLabels);
}
