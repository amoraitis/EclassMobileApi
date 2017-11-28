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

<iframe frameborder="0" style="width:100%;height:473px;" src="https://www.draw.io/?lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&title=Untitled%20Diagram.xml#R7VvbbuM2EP0aA9sCKURdfHmMnXi3QNoGm6TtPtISbbFLiSpFx%2FF%2BfYcSKVuilBhZxw66MoxEHI54mXM4nKHkgTdLnj4KnMW%2F8YiwgetETwPvauC6I9eHv0qw1QIUlIKVoFEpQjvBHf1GtNDR0jWNSF5TlJwzSbO6MORpSkJZk2Eh%2BKautuSs3muGV8QS3IWY2dK%2FaCTjUjoOnJ38E6Gr2PSMHF2zwOHXleDrVPc3cL1l8SmrE2za0vp5jCO%2B2RN51wNvJjiX5VXyNCNMmdaYrbxv3lFbjVuQVB50wwiHCxyM%2FDEJvMlwcaFbeMRsrW1xHTKc55cZ1SOWW2MlGHymLtcJmwucwOV0E1NJ7jIcKvkGuAGyWCYMSgguC8sQ1bcDpWryqhDyhIb6muEFYdPKlDPOuICqlKeqj1wK%2FpUYIVjYKT5VjUFMdbikjO1paixAzlM5xwlliqB%2FEhHhFGuxZiNydLmtI8zoKgVZCGYmUDm17W4MSYQkT3sijcNHwhMixRZUdK2hxLZe3Oz4NzLLI97jnm9IiTXnV1XLO9zhQkN%2FIA1ciwb3sP5sBmxownCByh7KhRW1UgFtTFl0g7d8rcacSwDWlKYxF%2FQb6GNzM1QLg4E7bECoGbC76U41prsRJIfbbg0GqBLd4FxWLGMMZzldFINTKgkWK5pOuZQ8MbzUs5p3kOf0LH5%2FbEWoQdfAt%2FiKhi18Rd5b8NWz%2BDpwYZrO74VbulT7xxZ8VZO%2FMFlZ524dFo1VCwWNTRlZqhaU4SjsH5daLLnyfDk4QpqubgqdK38n%2Bazt4Xe4TA7tLVnBqphGEUkL1kks8aJaVRmnqSxsGEzhC1adOb8EgwDmNYMy2pXhq9QFkCOF%2BWFaoE5gVWyIWhltfOhyCC8TxBBieBgfjN5R6eB30OGGpl97OpyDDoF7RjoMO%2Bhwr0jQ0%2BEMdBiNz0iHSQcdwACymKNixB%2BLf1Rm0XPiZJxAzoEhxDFIEUQumgSLaOxEeIkQuggsUsz4WuS2V%2Bhj3j7mNYS1c7RWwo69bsLq3j6Ds8HpCiCquhsG9e6ClpRwcsDywAwskGJJpgqS3Fol1URft3C6Ntdd6A3gggfrXemhrrTTN73L4Nsa7aiDEL9e9XQ4Dx1OGXzbJ0cDd8jUzLIa3sN%2F1%2BqQU%2B9FJTOc7An%2BFkZwSvlFAZ6q8%2FfqFFUuNOqqTjv5qk24Wun%2FRc8LIyjPMh%2FyQpmVKvNFUx1kWVMWCzV6c1BtOkLdfb5qtiWDq%2BnabStdNfzUcq56pqKuewvT3XARtem2TfPtRn1H8pzy9B5WdHr%2B0TzQg03yjpC%2FU3Hih3Ur%2FrPCa9ho%2F1QWFsXxbTtJVMx3RSSmLP%2Bg1R85GOgsyFwR5fa3mi4vj6exi7Q7%2F8YOsefS1XbSCCvdZvT5ibBHolodvM%2BHGR3bQUu02xna%2Bo1Yczixdgx31HaaG6Dv3zLs07rLNAVbhSRRw%2B8TsP9RAvZdLPWGdZZ6DjossEH%2BEWhqnxDoU0QqWZ%2FpHBzavsAA%2F90kNl2J7e4hwoOwH4r3WB8L61NmLV05K0QjoaCZhFCkX%2BEnQf2UTwbGXahjSW7XC0bzmEQ97ifB%2FaSn%2F8j27egNsK1inya6CWCiumnF7plI7%2Fno7pmIrS1Ci6ggYenZrnIIjOMjpRKNF5mQ05JLtL3IdJT3Qixcf%2F7hcD0Chu64gSGyMfTb1uYxMGzzyg0ISRpdqtc8ocSzwrmBZF6k8gWgUNrP5i2oTZ5jpfqHWpdE1gukL9q2zbEJwrCkj%2FW2nnlMdKt8dnfajvyGc4R1LUKi79phYDXkDsfPNwQJ8opIq6FXPEBqSfd%2FPN97hDWKmtiPT7dGW3Lh3s8eAUPXvBx5Cgzb8tv342fJE5V%2FK9ggBC1LX6pOYaZ7Var4RQN8qHeGWRaesRR1PWErnZ4JLdrebzmJl%2FcmozpL%2FNErvTwaP9%2FQ8by8Z%2F%2BQAHIr9eiqfK0m793FK84%2Fmy7%2F7cIyKO5%2BhVLiv%2Fulj3f9Hw%3D%3D"></iframe>

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