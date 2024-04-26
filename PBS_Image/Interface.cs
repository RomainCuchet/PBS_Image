using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppVisuals;
using ConsoleAppVisuals.Enums;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.PassiveElements;
using ConsoleAppVisuals.AnimatedElements;

namespace PBS_Image
{
    internal class Interface
    {
        static string racine = "../../../Images/";
        static string[] filters = ["sharpness", "box blur", "edge1", "edge2", "edge3","embossing", "reflect"];
        static string[] folders = ["Save/","Default/"];
        static int delay = 2500;
        
        public static string choose_folder(TableSelector folder_selector,Text text)
        {
            text.UpdateLines(new List<string>() { "SELECT IMAGE : Select a folder" });
            Window.ActivateElement(text);
            folder_selector.ClearLines();
            foreach(string folder in folders)
            {
                folder_selector.AddLine(new List<string>() { folder });
            }
            Window.ActivateElement(folder_selector);
            string response = folders[folder_selector.GetResponse().Value];
            Window.DeactivateElement(folder_selector);
            Window.DeactivateElement(text);
            return response;
        }  
        

        public static string choose_file(TableSelector file_selector, Text text,string path="")
        {
            text.UpdateLines(new List<string>() { "SELECT IMAGE : Select a file" });
            Window.ActivateElement(text);
            List<string> files = Directory.GetFiles(racine+path, "*.bmp").Select(Path.GetFileName).ToList();
            file_selector.ClearLines();
            int i = 0;
            while(i<files.Count && i<Console.BufferHeight-10)
            {
                file_selector.AddLine(new List<string>() { files[i] });
                i++;
            }
            Window.ActivateElement(file_selector);
            int value = file_selector.GetResponse().Value;
            Window.DeactivateElement(text);
            return racine+path+files[value]; ;
        }

        public static string choose_file_folder(TableSelector file_selector,TableSelector folder_selector,Text text)
        {
            return choose_file(file_selector,text,choose_folder(folder_selector,text));
        }

        public static void Home()
        {
            ScrollingMenu home_menu = new("Select an action", choices: [
                "README",
                "Filter an image",
                "Rotate an image",
                "Resize an image",
                "Mandelbroot fractal",
                "Julia fractal",
                "Hide Steganography",
                "Reveal Steganography",
                "Triple Hide Steganography",
                "Triple Reveal Steganography",
                "Huffman",
                "JPEG"
                ]);
            Dialog save_menu = new(new List<string> { "Select a name to save the image"},leftOption:"Make my own",rightOption:$"Save{Tools.get_counter(inc:false)+1}.bmp");
            Text text = new(new List<string>() {""});
            TableSelector file_selector = new(title:"choose",headers: ["images"]);
            TableSelector folder_selector = new(title: "choose", headers: ["folders"]);
            TableSelector filter_selector = new(title:"choose",headers: ["filter"]);
            Prompt prompt = new("Type a name for your file");
            IntSelector int_selector = new(question:"choose the integer part", min: 0, max:360,start:0,step:1);
            foreach(string filter in filters)
            {
                filter_selector.AddLine(new List<string>() { filter });
            }
            Title title = new("PBS Image");
            FakeLoadingBar loading = new();
            Window.AddElement(loading);
            Window.ActivateElement(loading);
            Window.DeactivateElement(loading);

            Window.AddElement(title);
            Window.AddElement(home_menu);
            Window.AddElement(text);
            Window.AddElement(filter_selector);
            Window.AddElement(file_selector);
            Window.AddElement(folder_selector);
            Window.AddElement(save_menu);
            Window.AddElement(int_selector);
            Window.AddElement(prompt);

            void SaveImage(MyImage image, string folder = "")
            {
                save_menu.UpdateRightOption($"Save{Tools.get_counter(inc: false) + 1}.bmp");
                Window.ActivateElement(save_menu);
                DialogOption option = save_menu.GetResponse().Value;
                if (option == DialogOption.Left)
                {
                    Window.DeactivateElement(save_menu);
                    Window.ActivateElement(prompt);
                    if (folder == "") image.save(file_name: prompt.GetResponse().Value, random_name: false);
                    else image.save(file_name: prompt.GetResponse().Value, random_name: false, folder: racine + folder);
                    Window.DeactivateElement(prompt);
                    text.UpdateLines(new List<string>() { "Succesfully saved the image" });
                    Window.ActivateElement(text);
                    Thread.Sleep(delay);
                    Window.DeactivateElement(text);
                }

                else if (option == DialogOption.Right)
                {
                    if (folder == "") image.save();
                    else image.save(folder: racine + folder);
                    text.UpdateLines(new List<string>() { "Succesfully saved the image" });
                    Window.ActivateElement(text);
                    Thread.Sleep(delay);
                    Window.DeactivateElement(text);
                }
            }

            string path = "";
            string stegano_folder = "Hide_Stegano/";
            string triple_stegano_folder = "Triple_Hide_Stegano/";
            while (true)
            {
                Window.ActivateElement(title);
                Window.ActivateElement(home_menu);
                var responseHomeMenu = home_menu.GetResponse();
                switch (responseHomeMenu?.Status)
                {
                    case Status.Selected:
                        switch (responseHomeMenu.Value)
                        {
                            case 0:
                                text.UpdateLines(new List<string>() { "README : This is a simple image processing software" ,"In order to prevent GUI error we automatically crop the number of files diplayed according to the size of the current window"});
                                Window.ActivateElement(text);
                                Thread.Sleep(6000);
                                Window.DeactivateElement(text);
                                break;
                            case 1:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Select a filter to apply" });
                                Window.ActivateElement(text);
                                Window.ActivateElement(filter_selector);
                                string filter = filters[filter_selector.GetResponse().Value];
                                Window.DeactivateElement(text);
                                path = choose_file_folder(file_selector,folder_selector,text);
                                MyImage image = new MyImage(path);
                                image = image.filter(filter);
                                SaveImage(image);
                                break;
                            case 2:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                path = choose_file_folder(file_selector, folder_selector, text);
                                image = new MyImage(path);
                                text.UpdateLines(new List<string>() { "Select an angle to rotate the image" });
                                Window.ActivateElement(text);
                                int_selector.UpdateQuestion("Chose the integer part of the angle");
                                int_selector.UpdateStart(0);
                                int_selector.UpdateMin(0);
                                int_selector.UpdateMax(360);
                                int_selector.UpdateStep(1);
                                Window.ActivateElement(int_selector);
                                float angle = int_selector.GetResponse().Value;
                                Window.DeactivateElement(int_selector);
                                int_selector.UpdateQuestion("Chose the decimal part of the angle");
                                int_selector.UpdateMax(9);
                                int_selector.UpdateStep(1);
                                Window.ActivateElement(int_selector);
                                angle += (float)(int_selector.GetResponse().Value*0.1);
                                Window.DeactivateElement(int_selector);
                                Window.DeactivateElement(text);
                                text.UpdateLines(new List<string>() { $"rotation angle {angle}°" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                image = image.rotate(angle, true);
                                SaveImage(image);
                                break;
                            case 3:
                                Window.DeactivateElement(title);
                                path = choose_file_folder(file_selector, folder_selector, text);
                                image = new MyImage(path);
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Select a factor to resize the image" });
                                Window.ActivateElement(text);
                                int_selector.UpdateQuestion("Chose the integer part of the resize factor");
                                int_selector.UpdateStart(0);
                                int_selector.UpdateMin(0);
                                int_selector.UpdateMax(8);
                                int_selector.UpdateStep(1);
                                Window.ActivateElement(int_selector);
                                float factor = int_selector.GetResponse().Value;
                                Window.DeactivateElement(int_selector);
                                int_selector.UpdateQuestion("Chose the décimal part of the resize factor");
                                int_selector.UpdateMax(9);
                                int_selector.UpdateStep(1);
                                Window.ActivateElement(int_selector);
                                factor += (float)(int_selector.GetResponse().Value * 0.1);
                                Window.DeactivateElement(int_selector);
                                Window.DeactivateElement(text);
                                text.UpdateLines(new List<string>() { $"resize factor {factor}" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                image = image.resize(factor);
                                SaveImage(image);
                                break;
                            case 4:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                int_selector.UpdateQuestion("Chose the size of the fractale");
                                int_selector.UpdateMax(4000);
                                int_selector.UpdateStart(1000);
                                int_selector.UpdateMin(1000);
                                int_selector.UpdateStep(100);
                                Window.ActivateElement(int_selector);
                                int size = int_selector.GetResponse().Value;
                                Window.DeactivateElement(int_selector);
                                image = new Mandelbrot(size,size).create();
                                SaveImage(image);
                                break;
                            case 5:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                int_selector.UpdateQuestion("Chose the size of the fractale");
                                int_selector.UpdateMax(4000);
                                int_selector.UpdateStart(1000);
                                int_selector.UpdateMin(1000);
                                int_selector.UpdateStep(100);
                                Window.ActivateElement(int_selector);
                                size = int_selector.GetResponse().Value;
                                Window.DeactivateElement(int_selector);
                                SaveImage(new Julia(size, size, 0.285, 0.01).create());
                                break;
                            case 6:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Select an image to hide" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                string image_path = choose_file_folder(file_selector, folder_selector, text);
                                MyImage image_to_hide = new MyImage(image_path);
                                Window.DeactivateElement(text);
                                text.UpdateLines(new List<string>() { "Select an image to hide the image in" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                string image_to_hide_path = choose_file_folder(file_selector, folder_selector, text);
                                MyImage image_to_hide_in = new MyImage(image_to_hide_path);
                                if (image_to_hide.width > image_to_hide_in.width || image_to_hide.height > image_to_hide_in.height)
                                {
                                    image_to_hide_in = image_to_hide_in.resize(Math.Max((double)image_to_hide.width / image_to_hide_in.width, (double)image_to_hide.height / image_to_hide_in.height) + 0.1);
                                }
                                try { image_to_hide_in.HideImage(image_to_hide); }
                                catch (Exception ex)
                                {
                                    text.UpdateLines(new List<string>() { $"ERROR : {ex.Message} "});
                                    Window.ActivateElement(text);
                                    Thread.Sleep(delay);
                                    Window.DeactivateElement(text);
                                }

                                Window.DeactivateElement(text);
                                text.UpdateLines(new List<string>() { $"Image will be available in {stegano_folder}" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                SaveImage(image_to_hide_in, stegano_folder);
                                Window.DeactivateElement(text);
                                break;
                            case 7:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Select an image to reveal" });
                                Window.ActivateElement(text);
                                string image_to_reveal_path = choose_file(file_selector, text, stegano_folder);
                                MyImage hidden = new MyImage(image_to_reveal_path);
                                hidden = hidden.GetHiddenImage();
                                SaveImage(hidden);
                                break;
                            case 8:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Select the first image to hide" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                image_path = choose_file_folder(file_selector, folder_selector, text);
                                MyImage image1_to_hide = new MyImage(image_path);
                                text.UpdateLines(new List<string>() { "Select the second image to hide" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                image_path = choose_file_folder(file_selector, folder_selector, text);
                                MyImage image2_to_hide = new MyImage(image_path);
                                text.UpdateLines(new List<string>() { "Select the third image to hide" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                image_path = choose_file_folder(file_selector, folder_selector, text);
                                MyImage image3_to_hide = new MyImage(image_path);
                                Window.DeactivateElement(text);
                                text.UpdateLines(new List<string>() { "Select an image to hide the image in" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                image_to_hide_in = new MyImage(choose_file_folder(file_selector, folder_selector, text));
                                if (image1_to_hide.width > image_to_hide_in.width || image1_to_hide.height > image_to_hide_in.height ||
                                    image2_to_hide.width > image_to_hide_in.width || image2_to_hide.height > image_to_hide_in.height ||
                                    image3_to_hide.width > image_to_hide_in.width || image3_to_hide.height > image_to_hide_in.height)
                                {
                                    double maxRatioWidth = Math.Max(Math.Max((double)image1_to_hide.width / image_to_hide_in.width, (double)image2_to_hide.width / image_to_hide_in.width), (double)image3_to_hide.width / image_to_hide_in.width);
                                    double maxRatioHeight = Math.Max(Math.Max((double)image1_to_hide.height / image_to_hide_in.height, (double)image2_to_hide.height / image_to_hide_in.height), (double)image3_to_hide.height / image_to_hide_in.height);
                                    double maxRatio = Math.Max(maxRatioWidth, maxRatioHeight);
                                    maxRatio += 0.1;
                                    image_to_hide_in = image_to_hide_in.resize(maxRatio);
                                }
                                try
                                {
                                    image_to_hide_in.HideTripleImage(image1_to_hide,image2_to_hide,image3_to_hide);
                                    Window.DeactivateElement(text);
                                    text.UpdateLines(new List<string>() { $"Image will be available in {triple_stegano_folder}" });
                                    Window.ActivateElement(text);
                                    Thread.Sleep(delay);
                                    Window.DeactivateElement(text);
                                    SaveImage(image_to_hide_in, triple_stegano_folder);
                                    Window.DeactivateElement(text);
                                }
                                catch (Exception ex)
                                {
                                    text.UpdateLines(new List<string>() { $"ERROR : {ex.Message} " });
                                    Window.ActivateElement(text);
                                    Thread.Sleep(5000);
                                    Window.DeactivateElement(text);
                                }
                                break;
                            case 9:
                                Window.DeactivateElement(title);
                                Window.DeactivateElement(home_menu);
                                text.UpdateLines(new List<string>() { "Select an image to reveal" });
                                Window.ActivateElement(text);
                                image = new MyImage(choose_file(file_selector, text, triple_stegano_folder));
                                (image1_to_hide,image2_to_hide,image3_to_hide) = image.GetTripleHiddenImage();
                                SaveImage(image1_to_hide);
                                SaveImage(image2_to_hide);
                                SaveImage(image3_to_hide);
                                break;
                            default:
                                text.UpdateLines(new List<string>() { "Not implemented to the interface, consult Demo" });
                                Window.ActivateElement(text);
                                Thread.Sleep(delay);
                                Window.DeactivateElement(text);
                                break;
                        }
                        break;
                    case Status.Escaped:
                    case Status.Deleted:
                        Window.Close();
                        break;
                }
            }
        }


    }
}
