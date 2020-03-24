﻿def ExportConcordance(pathArg, productionSetNameArg, exportPathArg, metadataProfileArg)

    
    
    the_case = utilities.case_factory.open(pathArg)

    productionSet = the_case.findProductionSetByName(productionSetNameArg)

    if productionSet == nil

        puts "Could not find production set with name '#{:productionSetNameArg.to_s}'"

    else
        batchExporter = utilities.createBatchExporter(exportPathArg)

        batchExporter.addLoadFile("concordance",{
        :metadataProfile => metadataProfileArg
		})

        batchExporter.addProduct("native", {
        :naming=> "full",
        :path => "Native"
        })

        batchExporter.addProduct("text", {
        :naming=> "full",
        :path => "Text"
        })


        puts 'Starting export.'
        batchExporter.exportItems(productionSet)        
        puts 'Export complete.'

    end

    the_case.close
    
    
end