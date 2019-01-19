require "rake"
require "fileutils"

##
# Run opencensus hello world for given language, feature, and request type,
#
# @param [String, Symbol] language The name of the language to run in.
#   Acceptable values are: csharp, go, java, nodejs, php, or ruby
# @param [String, Symbol] feature The opencensus feature to run hello world for.
#   Acceptable values are: stats, tracing-http, tracing-grpc, http, grpc
# @example bundle exec rake oc[ruby,grpc]
task :oc, [:language, :feature] do |_, args|
  FileUtils.mkdir_p "downloads"
  language, feature = verify_args args

  if language == :all
    LanguageCommands.commands.each do |lang, cmds|
      [:grpc, :http, :stats].each do |feat|
        Dir.chdir "#{lang}/#{feat}" do
          cmds.each { |cmd| `#{cmd}` }
        end
      end
    end
  else
    cmds = LanguageCommands.select_command language
    Dir.chdir "#{language}/#{args[:feature]}" do
      cmds.each { |cmd| `#{cmd}` }
    end
  end
  FileUtils.rm_rf "downloads"
end

def verify_args args
  language = args[:language]
  feature = args[:feature].split("tracing-").last
  return :all, nil if language == "all"
  if language.nil? || LanguageCommands.commands[language.to_sym].nil?
    raise ArgumentError.new "Invalid language option: #{language}. Valid "\
      "options are csharp, go, java, nodejs, php, python, and ruby."
  elsif feature.nil? || ![:grpc, :http, :stats].include? feature.to_sym
    raise ArgumentError.new "Invalid feature option: #{feature}. Valid "\
      "options are grpc, http, or stats."
  end
  language.to_sym, feature.to_sym
end

class LanguageCommands
  def self.select_command language
    cmds = self.commands[language.to_sym]
    if cmds.nil?
      FileUtils.rm_rf "downloads"
      raise ArgumentError.new "Invalid language option: #{language}. Valid "\
        "options are csharp, go, java, nodejs, php, python, and ruby."
    end
    cmds
  end

  def self.commands
    {
      csharp: ["dotnet run"],
      go: ["go run main.go"],
      java: ["mvn package", "mvn exec:java"],
      nodejs: ["npm install", "npm start"],
      php: ["echo write php commands"],
      python: ["echo write python commands"],
      ruby: ["bundle update", "bundle exec rake main.rb"],
      all: []
    }
  end
end
