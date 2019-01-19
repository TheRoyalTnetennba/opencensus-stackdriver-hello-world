require 'opencensus-stackdriver'
require 'google/cloud/vision'

OpenCensus.configure do |c|
  c.trace.default_sampler = OpenCensus::Trace::Samplers::AlwaysSample.new
  c.trace.exporter = OpenCensus::Trace::Exporters::Stackdriver.new
end

# Instantiates a client
image_annotator = Google::Cloud::Vision::ImageAnnotator.new

file_name = '../resources/demo-image.jpg'

response = image_annotator.label_detection image: file_name

OpenCensus::Trace.start_request_trace do
  OpenCensus::Trace.in_span 'my_task' do
    9.times do
      image_annotator.label_detection image: file_name
    end
  end
end

response.responses.each do |res|
  puts 'Labels:'
  res.label_annotations.each do |label|
    puts label.description
  end
end
