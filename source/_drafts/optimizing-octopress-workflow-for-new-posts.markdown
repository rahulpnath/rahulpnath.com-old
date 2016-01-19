---
layout: post
title: "Optimizing Octopress Workflow for New Posts"
comments: true
categories:
- Productivity
- Blogging
tags: 
thisIsStillADraft:
keywords: 
description: 
---
Over the past month I had thought of migrating this blog to a static site generator that is faster than the current one, Octopress. Lack of a good workflow for creating new posts and slower build times were the main reasons. Though the build is not very slow, I am the kind of person when writing post want to see often, how it looks like on the real site. With the current number of posts it takes around 40-50 seconds to build the entire site and it makes me to wander off to something else while the build is happening - at times it takes a long time to get back to writing!. But migrating to a new platform has a lot of challenges and time-consuming and I did not want to invest my time in that, so though of looking out for ways to optimize the current process. A bit of googling and playing around with Ruby, solved both of the major issues and I have an improved workflow!

#### **Draft workflow** ####
I was lucky to find this [post](http://neverstopbuilding.com/how-to-enhance-your-octopress-draft-and-heroku-deploy-process) which handled most of the draft workflow process. Most of the code below is used from there with a very few minor additions. Newer versions of Jekyll support [working with drafts](http://jekyllrb.com/docs/drafts/) and uses the '*--drafts*' switch to build the drafts (instead of using published flag), that are in '*_drafts*' folder. Drafts are posts which does not have date's, so I add in a placeholder text, '*thisIsStillADraft*', in the yaml front matter of the post which will later be replaced with the post publish date. 


``` ruby Rake new_draft
# usage rake new_draft[my-new-draft] or rake new_draft['my new draft']
desc "Begin a new draft in #{source_dir}/#{drafts_dir}"
task :new_draft, :title do |t, args|
  if args.title
    title = args.title
  else
    title = get_stdin("Enter a title for your post: ")
  end
  raise "### You haven't set anything up yet. First run `rake install` to set up an Octopress theme." unless File.directory?(source_dir)
  mkdir_p "#{source_dir}/#{drafts_dir}"
  filename = "#{source_dir}/#{drafts_dir}/#{title.to_url}.#{new_post_ext}"
  if File.exist?(filename)
    abort("rake aborted!") if ask("#{filename} already exists. Do you want to overwrite?", ['y', 'n']) == 'n'
  end
  puts "Creating new draft: #{filename}"
  open(filename, 'w') do |post|
    post.puts "---"
    post.puts "layout: post"
    post.puts "title: \"#{title.gsub(/&/,'&amp;')}\""
    post.puts "comments: true"
    post.puts "categories: "
    post.puts "tags: "
    post.puts "thisIsStillADraft:"
    post.puts "keywords: "
    post.puts "description: "
    post.puts "---"
  end
  system %{cmd /c "start #{filename}"}
end
```

The publish draft task just asks for the post to publish and replaces the placeholder text with the current date time. Also it moves the post from the '*_drafts*' folder to the '*_posts*' folder with the file name appended with the date time. Since I run this just before deploying a post, the date on the post will be the correct date, and not the date I started writing the post (usually  post spans over multiple days).  


``` ruby Rake publish_draft
# usage rake publish_draft
desc "Select a draft to publish from #{source_dir}/#{drafts_dir} on the current date."
task :publish_draft do
  drafts_path = "#{source_dir}/#{drafts_dir}"
  drafts = Dir.glob("#{drafts_path}/*.#{new_post_ext}")
  drafts.each_with_index do |draft, index|
    begin
      content = File.read(draft)
      if content =~ /\A(---\s*\n.*?\n?)^(---\s*$\n?)/m
        data = YAML.load($1)
      end
    rescue => e
      puts "Error reading file #{draft}: #{e.message}"
    rescue SyntaxError => e
      puts "YAML Exception reading #{draft}: #{e.message}"
    end
    puts "  [#{index}]  #{data['title']}"
  end
  puts "Publish which draft? "
  answer = STDIN.gets.chomp
  if /\d+/.match(answer) and not drafts[answer.to_i].nil?
    mkdir_p "#{source_dir}/#{posts_dir}"
    source = drafts[answer.to_i]
    filename = source.gsub(/#{drafts_path}\//, '')
    dest = "#{source_dir}/#{posts_dir}/#{Time.now.strftime('%Y-%m-%d')}-#{filename}"
    puts "Publishing post to: #{dest}"
    File.open(source) { |source_file|
      contents = source_file.read
      contents.gsub!(/^thisIsStillADraft:$/, "date: #{Time.now.strftime('%Y-%m-%d %H:%M')}")
      File.open(dest, "w+") { |f| f.write(contents) }
    }
    FileUtils.rm(source)
  else
    puts "Index not found!"
  end
end
```

With these two new rake tasks, I can now create as many draft posts at a time and publish them once ready. 

#### **Improving the build time** ####

Jekyll build command options provides a switch, '*configuration*', that allows to pass a configuration file instead of using '*_config.yml*'. In the configuration file we can specify a 'exclude' option to exclude the directories and/or files from the build. I created a new task for building only the current drafts, by specifying the '*--drafts*' switch and a dynamically generated configuration file, *_previewconfig.yml*, which excludes the '*_posts*' folder. This dramatically increases the build time to near real time!. While writing new posts I do not want to see any already published posts. I did not want to use the the '*rake isolate*' task that is already present in the  rakefile, as that does not integrate with the draft workflow and unnecessarily moves all the posts to a temporary place. You can also add the dynamically generated configuration file name to the *.gitignore* as I do not delete it. 

``` ruby
desc "preview the site in a web browser with all the draft posts"
task :previewdrafts do
  raise "### You haven't set anything up yet. First run `rake install` to set up an Octopress theme." unless File.directory?(source_dir)
  puts "Starting to watch source with Jekyll and Compass. Starting Rack on port #{server_port}"
  system "compass compile --css-dir #{source_dir}/stylesheets" unless File.exist?("#{source_dir}/stylesheets/screen.css")
  File.open("_config.yml") { |source_file|
      contents = source_file.read
      File.open("_previewconfig.yml", "w+") { |f|
      f.write(contents)
      f.puts("exclude: [\"#{posts_dir}\"]") 
      }
    }
    
  jekyllPid = Process.spawn({"OCTOPRESS_ENV"=>"preview"}, "jekyll build --watch --drafts --config _previewconfig.yml")
  compassPid = Process.spawn("compass watch")
  rackupPid = Process.spawn("rackup --port #{server_port}")

  trap("INT") {
    [jekyllPid, compassPid, rackupPid].each { |pid| Process.kill(9, pid) rescue Errno::ESRCH }
    exit 0
  }

  [jekyllPid, compassPid, rackupPid].each { |pid| Process.wait(pid) }
end
```

#### **Dropbox integration** ####

At times, I have started to draft blog posts while commuting to work from my mobile device, so I wanted to sync my draft posts to [Dropbox](https://db.tt/bvYw3pL6), so that I can edit it from my [mobile phone](http://www.rahulpnath.com/blog/review-two-months-and-counting-android-and-nexus-5/). Apps like [MarkDrop](https://play.google.com/store/apps/details?id=net.keepzero.markdrop&hl=en)/[JotterPad](https://play.google.com/store/apps/details?id=net.keepzero.markdrop&hl=en) integrates with Dropbox and supports Markdown editing. I set up a drafts folder on my Dropbox folder on laptop, which is automatically synced using the [Dropbox application](https://www.dropbox.com/install). I then used [Mklink](https://technet.microsoft.com/en-us/library/cc753194.aspx) to create a symbolic link from the folder on Dropbox to my drafts folder in my blog repository. Whenever a new draft post is added, it gets automatically inserted into the Dropbox folder, which will then be synced to cloud and available for edit on my mobile phone too. (Part of this post is written from my mobile!)

``` text
mklink /D "C:\blog\_drafts" "C:\dropbox\_drafts"
``` 

#### **Cmder integration** ####

Alias shortcuts
