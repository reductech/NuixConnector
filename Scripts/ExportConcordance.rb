﻿#ExportConcordance

requiredNuixVersion = '7.2'
if Gem::Version.new(NUIX_VERSION) < Gem::Version.new(requiredNuixVersion)
	puts "Nuix Version is #{NUIX_VERSION} but #{requiredNuixVersion} is required"
	exit
end

requiredFeatures = Array['EXPORT_ITEMS', 'PRODUCTION_SET']
requiredFeatures.each do |feature|
	if !utilities.getLicence().hasFeature(feature)
		puts "Nuix Feature #{feature} is required but not available."
		exit
	end
end

require 'optparse'
params = {}
OptionParser.new do |opts|
	opts.on('--pathArg1 [ARG]') do |o| params[:pathArg1] = o end
	opts.on('--exportPathArg1 [ARG]') do |o| params[:exportPathArg1] = o end
	opts.on('--productionSetNameArg1 [ARG]') do |o| params[:productionSetNameArg1] = o end
end.parse!


def ExportConcordance(utilities,pathArg,exportPathArg,productionSetNameArg)

    the_case = utilities.case_factory.open(pathArg)

    productionSet = the_case.findProductionSetByName(productionSetNameArg)

    if productionSet == nil
        puts "Could not find production set with name '#{productionSetNameArg.to_s}'"
    elsif productionSet.getProductionProfile == nil
        puts "Production set '#{productionSetNameArg.to_s}' did not have a production profile set."
    else
        batchExporter = utilities.createBatchExporter(exportPathArg)


        puts 'Starting export.'
        batchExporter.exportItems(productionSet)
        puts 'Export complete.'

    end

    the_case.close
end


ExportConcordance(utilities, params[:pathArg1], params[:exportPathArg1], params[:productionSetNameArg1])
puts '--Script Completed Successfully--'
