// See https://aka.ms/new-console-template for more information
using System;
using System.CommandLine;

//אפשרות של קליטת מיקום הקובץ
var bundleOutput = new Option<FileInfo>("--output", "File path and name");
//אפשרות שלוקחת את כל הקבצים בלי דוקא קבצי תכנות 
var bundleLanguage = new Option<string>("--language", "bundle all the files into the file");
//אם נשתמש באפשרות זו ניתוב הקובץ יכתב כהערה לפני תוכנו
var bundleNote = new Option<bool>("--note", "write the makor");
//כתיבת שם יוצר הקובץ בראש הדף
var bundleAuthor = new Option<string>("--author", "write the author");
//כתיבת שם יוצר הקובץ בראש הדף
var bundleEmptyLines = new Option<bool>("--emptyLine", "write the author");
//אפשרות של מיון הקובץ לפי סוג הקובץ
var bundleSortByType = new Option<bool>("--sort", "write the author");
// הגדרת הפעולה בנדל שהיא מכווצת את הקבצים שבפרטיקט לקובץ אחד
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
//bundle הוספת האפשרות לפקודת ה
bundleCommand.AddOption(bundleOutput);
bundleCommand.AddOption(bundleLanguage);
bundleCommand.AddOption(bundleNote);
bundleCommand.AddOption(bundleAuthor);
bundleCommand.AddOption(bundleEmptyLines);
bundleCommand.AddOption(bundleSortByType);

//הצבת ערך ברירת מחדל
bundleOutput.SetDefaultValue(true);
bundleLanguage.IsRequired = true;
bundleNote.SetDefaultValue(false);
bundleAuthor.SetDefaultValue("");
bundleEmptyLines.SetDefaultValue(false);
bundleSortByType.SetDefaultValue(false);

//הוספת כינוי
bundleOutput.AddAlias("-o");
bundleLanguage.AddAlias("-l");
bundleNote.AddAlias("-n");
bundleAuthor.AddAlias("-a");
bundleEmptyLines.AddAlias("-e");
bundleSortByType.AddAlias("-s");

bundleCommand.SetHandler((output, language, note, author, emptyLine, sort) =>
{
    bool isIncludeFile;//סימון השפות להכנסה
    //מערך לסיומות האפשריות
    string[] typeLang = { "css", "js", "html", "py", "java", "cs", "sql"};
    try
    {
        //מערך ניתוב של כל הקבצים כולל קבצים בתוך תיקיות
        var filesInDirectory = Directory.GetFiles(output.Directory.FullName, "*", SearchOption.AllDirectories);

        using (StreamWriter writer = File.CreateText(output.FullName))
        {
            if (author != "")//אם המשתמש הכניס את שם יוצר הקובץ  
                writer.WriteLine("// the author is: " + author);

            //מיון על ידי שם הקובץ - sortאם לא הכניסו ערך ב
            if(!sort)      
                filesInDirectory=filesInDirectory.OrderBy(o=>Path.GetFileNameWithoutExtension(o)).ToArray();//ע"י שם הקובץ
            else//מיון על ידי םוג הקובץ - sortאם הכניסו ערך ב
                filesInDirectory = filesInDirectory.OrderBy(o => Path.GetExtension(o)).ToArray();//ע"י סוג הקובץ

            foreach (var nituv in filesInDirectory)//ניתוב בודד מתוך כולם
            {
                isIncludeFile = false;
                var fileExtension = Path.GetExtension(nituv).ToLower();//סוג הקובץ
                if (language == "all")//אם המשתמש הכניס את כל השפות
                {
                    isIncludeFile = true;
                }
                else
                {
                    //חלוקת שפות התכנות למערך ע"י רווח
                    string[] langs = language.Split(",");
                    //מעבר על השפות שהמשתמש הכניס
                    foreach (var lang in langs)
                    {
                        // בודק האם לכלול את הקובץ לפי מה שהמשתמש הכניס
                        if ((typeLang.Contains(lang)) //אם המשתמש הכניס שפה שאינה במערך
                               && fileExtension == "." + lang)// 
                        {
                            isIncludeFile = true;
                        }
                    }
                }
                if (isIncludeFile)//אם זהו סוג קובץ רצוי
                {
                     //ערך note אם  הכניסו ב 
                     if (note)
                         writer.WriteLine($"// {nituv}");//כתיבת ההערה עם הניתוב

                      var fileContent = File.ReadAllText(nituv);//תוכן של קובץ אחד

                      //בדיקה האם המשתמש הכניס את האפשרות של הסרת שורות ריקות
                      if (emptyLine)
                      {
                         foreach (var line in fileContent.Split('\n'))
                         {
                             if (!string.IsNullOrWhiteSpace(line))
                                 writer.WriteLine(line);
                             else
                                 continue;
                         }
                      }
                      else//אם המשתמש לא בחר באפשרות של שורות ריקות
                           writer.WriteLine(fileContent);//כתיבת תוכן הקובץ
                }
                
            }
            Console.WriteLine("Text was written to the file");
        }
    }
    catch(DirectoryNotFoundException ex)
    {
        Console.WriteLine("Error:file path is invalid");
    }
    catch(NullReferenceException ex)
    {
        Console.WriteLine("Error:You didn't consider all the options");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}, bundleOutput, bundleLanguage, bundleNote, bundleAuthor, bundleEmptyLines, bundleSortByType);

var rootCommand = new RootCommand("Root Command for File Bundler CLI");
rootCommand.AddCommand(bundleCommand);

//create-rsp  יצירת פקודת 
var create_rspCommand = new Command("create-rsp", "questions for the clints");
create_rspCommand.SetHandler(() =>
{
    //שאלות למשתמש
    string s = "bundle";
    Console.WriteLine("הכנס את מיקום ושם הקובץ");
    
    var outputFile=Console.ReadLine();
    s += " --output " + outputFile;
    Console.WriteLine("על מנת לבחור בכל השפות all הכנס את השפות הרצויות ניתן להזין את המילה ");
    s += " --language " + Console.ReadLine();
    Console.WriteLine("?האם לרשום את מקור הקובץ כהערה");
    s += " --note " + Console.ReadLine();
    Console.WriteLine("הזן את שם יוצר הקובץ");
    s += " --author " + Console.ReadLine();
    Console.WriteLine("?האם למחוק שורות ריקות");
    s += " --emptyLine " + Console.ReadLine();
    Console.WriteLine("?האם רצונך שהמערך ימויין על ידי סוג הקובץ ולא על ידי שם הקובץ");
    s += " --sort " + Console.ReadLine();
    Console.WriteLine("fib @file.rsp :כדי להריץ יש לכתוב את הפקודה הבאה");
    File.WriteAllText("file.rsp",s);
});
create_rspCommand.AddCommand(bundleCommand);

rootCommand.AddCommand(create_rspCommand);
rootCommand.InvokeAsync(args);