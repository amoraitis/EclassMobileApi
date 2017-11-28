# [Eclass Mobile API](https://dev.openeclass.org/projects/openeclass/wiki/%CE%A7%CF%81%CE%AE%CF%83%CE%B7_%CF%84%CE%BF%CF%85_Mobile_API) Client for C#(.net core)
[![NuGet version](https://badge.fury.io/nu/EclassApi.svg)](https://badge.fury.io/nu/EclassApi)
#### How to:

<pre><code class='language-cs'>
//Init an Eclass Session for eclass.aueb.gr
EclassUser eclassUser = new EclassUser("aueb");
//Start a session with given usename and pass
await eclassUser.Start("Username", "Password");
//Print for all User Courses: Course Name, Course ID, Tools by Name
eclassUser.UserCourses.ForEach(course => {
                    Console.WriteLine(course.Name + " " + course.ID);
                    course.Tools.ForEach(tool=>Console.WriteLine(tool.Name));
                });
eclassUser.DestroySession();
</code></pre>

###### Attention
```
Property Tool.Content is type of Object, but you should use it as:
case docs: List<HtmlNode> of 2 elements
case description: HtmlNode
case courseDescription: HtmlNode
case announcements: List<Announcements>
```

#### Class Diagram for Avalaible Classes and fields
![ClassDiagram.svg](https://raw.githubusercontent.com/amoraitis/EclassMobileApi/master/EclassApi/ClassDiagram.svg)

#### Roadmap-Features

- [x] Login(token)
- [x] Tools for Course(specific)
- [x] EclassUser Data(username, pass, uid)
- [x] Course(courseID, Token)
	- [x] Announcements

	- [x] Directories(a list of 2 HtmlNode elements)
    
    - [x] Course Description(HtmlNode element)
    
    - [x] Description(HtmlNode element)
- [x] [**Nuget package**](https://www.nuget.org/packages/EclassApi/)
- [x] Documentation

HtmlNode [Documentation](http://www.nudoq.org/#!/Packages/HtmlAgilityPack/HtmlAgilityPack/HtmlNode)