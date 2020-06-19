# GoogleDriveDataScraper <img src="https://github.com/MyAlexro/GoogleDriveDataScraper/blob/master/GoogleDriveDataScraperIcon.png" alt="Imageconverter logo" width="80px">
Get a list of all(or some) of the files in your Drive, useful if you have a large backup folder but when you compare it with your local folder, *somehow*, some files are missing.
This application has been written using the framework .NET Core 3.1

### Usage

Go to the folder where your files are, copy the link in the url bar and paste that link in another tab, after making the files you want in your final list load(sometimes they'll load only by selecting them) press F12, go to the Elements tab in the opened window, copy the Body element, paste it in a txt file and save it. Finally drop it on the GoogleDriveDataScraper executable or use it normally

### Command line option
- **-scrape** (path to the txt file containing the body tag): Finds the names of the files in your Drive
- **-save**: Saves the list of the files in your Drive in a txt file on the desktop
