#$filter = "**/*.{tpp,hpp,h,cpp,c}"
$filter = "**/*.{cs}"

def get_files(directory_name = '.')
	Dir.chdir(directory_name)
	Dir[$filter].entries
end

def count(files)
	count = 0
	
	files.each do |file_name|
		next if File.directory?(file_name) # skip directories
		
		f = File.open(file_name)
		
		while f.gets
			count = count + 1
		end
	end

	return count
end

if ARGV.length == 1
	count = count(get_files(ARGV.first))/2
else
	count = count(get_files)/2
end

puts "Total lines of code: #{count}\nTotal KLOC's: #{count / 1000.0}"

