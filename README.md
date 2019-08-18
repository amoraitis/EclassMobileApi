# [Eclass Mobile API](https://dev.openeclass.org/projects/openeclass/wiki/%CE%A7%CF%81%CE%AE%CF%83%CE%B7_%CF%84%CE%BF%CF%85_Mobile_API) Client for C#(.net core)
[![NuGet version](https://badge.fury.io/nu/EclassApi.svg)](https://badge.fury.io/nu/EclassApi)
[![Build status](https://ci.appveyor.com/api/projects/status/v0ef1a14et07k0td?svg=true)](https://ci.appveyor.com/project/amoraitis/eclassmobileapi)

It will take 12 seconds to login and download(fill) 10 courses.

#### How to:

<pre><code class='language-cs'>
//Init an Eclass Session for eclass.aueb.gr
EclassUser eclassUser = new EclassUser("aueb");
//Start a session with given usename and pass
await eclassUser.Start("Username", "Password");

//Add tools apart from announcements
await user.UserCourses.AddToolsAsync();

//Add Announcements
await user.AddAnnouncementsAsync();

//Print for all User Courses: Course Name, Course ID, Tools by Name
eclassUser.UserCourses.ForEach(course => {
                    Console.WriteLine(course.Name + " " + course.ID);
                    course.ToolViewModel.Tools.ForEach(tool=>Console.WriteLine(tool.Name));
                });
eclassUser.DestroySession();
</code></pre>

#### Class Diagram for Avalaible Classes and fields
Deprecated see [v1.1.15 branch](https://github.com/amoraitis/EclassMobileApi/tree/v1.1.15+improvements)

#### Roadmap-Features

- [x] Login(token)
- [x] Tools for Course(specific)
- [x] EclassUser Data(username, pass, uid)
- [x] Course(courseID, Token)
	- [x] Announcements

	- [x] Directories-Docs(2 links, the one is the download link of the home directory of the course)
    
    - [x] Course Description(as a string)
    
    - [x] Description(as a string)
- [x] Logout
- [x] [**Nuget package**](https://www.nuget.org/packages/EclassApi/)
- [x] Documentation
