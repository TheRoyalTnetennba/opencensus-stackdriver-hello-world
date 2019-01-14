require 'sinatra'
require 'opencensus-stackdriver'
require "opencensus/trace/integrations/rack_middleware"
require "google/cloud/vision"

OpenCensus.configure do |c|
  c.trace.default_sampler = OpenCensus::Trace::Samplers::AlwaysSample.new
  c.trace.exporter = OpenCensus::Trace::Exporters::Stackdriver.new
end

configure do
  use OpenCensus::Trace::Integrations::RackMiddleware
end

get '/' do
  image_annotator = Google::Cloud::Vision::ImageAnnotator.new

  OpenCensus::Trace.in_span "my_task" do |span|
    # 9.times do 
    #   image_annotator.label_detection(
    #     image: ENV['IMAGE'],
    #   )
    # end
  
    response = image_annotator.label_detection(
      image: ENV['IMAGE'],
    )
  
    response.responses.each do |res|
      res.label_annotations.each do |label|
        puts label.description
      end
    end
  end

  "done"
end