﻿require 'optparse'
#ExtractEntities
params = {}
OptionParser.new do |opts|
opts.on('--casePathArg0 ARG')
opts.on('--outputFolderPathArg0 ARG')
end.parse!(into: params)
puts params

def ExtractEntities(utilities,casePathArg,outputFolderPathArg)

    the_case = utilities.case_factory.open(casePathArg)

    puts "Extracting Entities:"

    entityTypes = the_case.getAllEntityTypes()

    results = Hash.new { |h, k| h[k] = Hash.new { [] } }

    entitiesText = "TypeDescription\tValue\tCount" #The headers for the entities file

    if entityTypes.length > 0
        allItems = the_case.searchUnsorted("named-entities:*")    

        allItems.each do |i|            
            entityTypes.each do |et|
                entities = i.getEntities(et)
                entities.each do |e|
                   results[et][e] =  results[et][e].push(i.getGuid())
                end
            end
        end

        puts "Found entities for #{allItems.length} items"        

        results.each do |et, values|
            totalCount = values.map{|x,y| y.length}.reduce(:+)
            entitiesText << "#{et}\t*\t#{totalCount}" #The total count for entities of this type
            currentText = "Value\tGuid" #The header for this types' file
            values.each do |value, guids|
                entitiesText << "#{et}\t#{value}\t#{guids.length}" #The row in the entities file
                guids.each do |guid|                
                    currentText << "#{value}\t#{guid}" #The row in this entity type file
                end				                
            end
            File.write(File.join(outputFolderPathArg, et + '.txt'), currentText)
        end
    else
        puts "Case has no entities"
    end

    File.write(File.join(outputFolderPathArg, 'Entities.txt'), entitiesText) #For consistency, file is written even if there are no entities

    the_case.close
end



ExtractEntities(utilities, params[:casePathArg0], params[:outputFolderPathArg0])
puts '--Script Completed Successfully--'
