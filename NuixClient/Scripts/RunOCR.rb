require 'optparse'

hash_options = {}
OptionParser.new do |opts|
  opts.banner = "Usage: your_app [options]"
  opts.on('-p [ARG]', '--path [ARG]', "Case Path") do |v|
    hash_options[:pathArg] = v
  end 
  opts.on('-o [ARG]', '--ocrProfile [ARG]', "OCR Profile Name") do |v|
    hash_options[:ocrProfileArg] = v
  end 
  opts.on('-h', '--help', 'Display this help') do 
    puts opts
    exit
  end

end.parse!

requiredArguments = [:pathArg] #ocrProfileArg is optional

unless requiredArguments.all? {|a| hash_options[a] != nil}
    puts "Missing arguments #{(requiredArguments.select {|a| hash_options[a] == nil}).to_s}"


else
    puts "Opening Case"
    
    the_case = utilities.case_factory.open(hash_options[:pathArg])

    searchTerm =
    "NOT flag:encrypted AND ((mime-type:application/pdf AND NOT content:*) OR (mime-type:image/* AND ( flag:text_not_indexed OR content:( NOT * ) )))"
    
    items = the_case.searchUnsorted(searchTerm).to_a

    puts "Running OCR on #{items.length} items"
    
    processor = utilities.createOcrProcessor()


    if hash_options[:ocrProfileArg] != nil
        ocrProfileStore = the_case.getOcrProfileStore()

        puts "Got profile store"

        profile = ocrProfileStore.getProfile(hash_options[:ocrProfileArg])

        if profile != nil
            processor.process(items, profile)
            puts "Items Processed"
        else
            puts "Could not find profile '#{hash_options[:ocrProfileArg]}'"
        end
    else
        processor.process(items)
        puts "Items Processed"
    end

    
    the_case.close
    puts "Case Closed"
    
end