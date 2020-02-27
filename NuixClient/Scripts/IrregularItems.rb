require 'optparse'

hash_options = {}
OptionParser.new do |opts|
  opts.banner = "Usage: your_app [options]"
  opts.on('-p [ARG]', '--path [ARG]', "Case Path") do |v|
    hash_options[:pathArg] = v
  end 
  opts.on('-h', '--help', 'Display this help') do 
    puts opts
    exit
  end

end.parse!

requiredArguments = [:pathArg]

unless requiredArguments.all? {|a| hash_options[a] != nil}
    puts "Missing arguments #{(requiredArguments.select {|a| hash_options[a] == nil}).to_s}"


else
    puts "Opening Case"
    
    the_case = utilities.case_factory.open(hash_options[:pathArg])

    puts "Generating Report:"
   
    fields = {    
        encrypted: "flag:encrypted",
        NonSearchablePDF: "mime-type:application/pdf AND NOT content:*",
        BadExtension: "flag:irregular_file_extension",
        Unrecognised: "kind:unrecognised",
        Unsupported: "NOT flag:encrypted AND has-embedded-data:0 AND ( ( has-text:0 AND has-image:0 AND NOT flag:not_processed AND NOT kind:multimedia AND NOT mime-type:application/vnd.ms-shortcut AND NOT mime-type:application/x-contact AND NOT kind:system AND NOT mime-type:( application/vnd.apache-error-log-entry OR application/vnd.git-logstash-log-entry OR application/vnd.linux-syslog-entry OR application/vnd.logstash-log-entry OR application/vnd.ms-iis-log-entry OR application/vnd.ms-windows-event-log-record OR application/vnd.ms-windows-event-logx-record OR application/vnd.ms-windows-setup-api-win7-win8-log-boot-entry OR application/vnd.ms-windows-setup-api-win7-win8-log-section-entry OR application/vnd.ms-windows-setup-api-xp-log-entry OR application/vnd.squid-access-log-entry OR application/vnd.tcpdump.record OR application/vnd.tcpdump.tcp.stream OR application/vnd.tcpdump.udp.stream OR application/vnd.twitter-logstash-log-entry OR application/x-pcapng-entry OR filesystem/x-linux-login-logfile-record OR filesystem/x-ntfs-logfile-record OR server/dropbox-log-event OR text/x-common-log-entry OR text/x-log-entry ) AND NOT kind:log AND NOT mime-type:application/vnd.ms-exchange-stm ) OR mime-type:application/vnd.lotus-notes )",
        TextNotIndexed: "flag:text_not_indexed",
        ImagesNotProcessed: "flag:images_not_processed",
        Poisoned: "flag:poison",
        Record: "mime-type:( filesystem/x-ntfs-mft OR filesystem/x-ntfs-logfile OR filesystem/x-ntfs-file-record OR filesystem/x-ntfs-index-record OR filesystem/x-ntfs-logfile-record OR filesystem/x-ntfs-usnjrnl OR filesystem/x-ntfs-usnjrnl-record OR filesystem/x-ntfs-vss-catalog OR filesystem/x-ntfs-vss-store ) OR ( path-mime-type:filesystem/drive AND ( ( path-name:\"[File System Root]\" AND name:( \"$AttrDef\" OR \"$Bitmap\" OR \"$BadClus\" OR \"$BadClus:$Bad\" OR \"$Boot\" OR \"$Extend\" OR \"$Secure\" OR \"$Secure:$SDS\" OR \"$UpCase\" OR \"$UpCase:$Info\" OR \"$Volume\" ) ) OR path-name:\"[File System Root]/$Extend\" ) )",
        UnrecognisedDeleted: "kind:unrecognised AND flag:deleted",
        NeedManualExamination: "kind:unrecognised AND NOT ( flag:deleted OR mime-type:( filesystem/x-ntfs-mft OR filesystem/x-ntfs-logfile OR filesystem/x-ntfs-file-record OR filesystem/x-ntfs-index-record OR filesystem/x-ntfs-logfile-record OR filesystem/x-ntfs-usnjrnl OR filesystem/x-ntfs-usnjrnl-record OR filesystem/x-ntfs-vss-catalog OR filesystem/x-ntfs-vss-store ) OR ( path-mime-type:filesystem/drive AND ( ( path-name:\"[File System Root]\" AND name:( \"$AttrDef\" OR \"$Bitmap\" OR \"$BadClus\" OR \"$BadClus:$Bad\" OR \"$Boot\" OR \"$Extend\" OR \"$Secure\" OR \"$Secure:$SDS\" OR \"$UpCase\" OR \"$UpCase:$Info\" OR \"$Volume\" ) ) OR path-name:\"[File System Root]/$Extend\" ) ) )",
        CodeTextFiles: "kind:unrecognised AND (content:(function OR def) AND IF)"

    }
    
    puts "OutputIrregular:reason\tcount"

    fields.each do |key, value|
        items = the_case.search(value)
        puts "OutputIrregular:#{key.to_s}\t#{items.length}"
        
        if items.length > 0
            puts "Output#{key.to_s}:path\tguid"
            items.each do |i|
                path = i.getPathNames().join("/")
                guid = i.getGuid()
                puts "Output#{key.to_s}:#{path}\t#{guid}"

        end

        
        end
    end

    the_case.close
    puts "Case Closed"
    
end