## DocXSwapper

This is a program for lazy people that don't want to fill word documents.<br>
You create a bunch of templates with placeholders all over and this will replace them with your given fields.

### Setup

**[1]** - Stash your templates in a folder. <br>
**[2]** - Add placeholders such as `...Name...` to the fields that you want to replace.<br>
**[3]** - Create a .txt file and define what you want to replace the placeholders with.<br>
Example:
``` Text
Name = Bob Marley
Address = 74 Reggae Street, Heaven
```
No need to add the placeholder prefix here, only the text. <br>
**[4]** - Configure DocXSwapper - Check the Config.conf file for all configurations.


**!! Known Issues !!** <br>
Sometimes the placeholder might get split into `...` `word` `...` instead of `...word...` and the program will not be able to find it.
In order to fix this, copy the placeholder's text into something like notepad, copy it from there and paste it back, this will force the editor to treat it as a whole e.g. `...word...`.


Config explained:
```text
templates_folder - here you put your templates path
exports_folder - where you want your results
sample_files_folder - where you'll be placing the samples
export_into_folders - export all templates into a folder with the sample name
placeholder_start - customize the start of the placeholder
placeholder_end - customize the end of the placeholder
print_replacements - useful for debug, it will print all replacelemnts done
```
<br>

Enjoy!