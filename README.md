# CcvSignIn

This is a WPF application that we can use at church if our online, children's sign-in system is not available for any reason. 

The application allows a list of children to be loaded from a CSV file and then the user can pick a child and sign them into one of a number of pre-determined children's environments. If a Dymo label printer is attached to the computer and is available for use then a configurable number of labels will be printed automatically for each child signed in - that's just the way it works in our church. The data is saved back to the input CSV file automatically.

This application is basic and not particularly well engineered, but it's only intended for use as an emergency backup on a handful of Sundays each year.

Possible enhancements:

+ Make the list of environments configurable
+ Pre-select or suggest an environment for a selected child (dependent on available input data)
+ Add medic alert flags to the printed labels (dependent on available input data)
+ Refactor (cleanup) the code!

