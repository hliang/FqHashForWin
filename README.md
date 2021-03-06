# FqHashForWin
A tool to generate MD5 checksum for sequencing data and verify integrity. It can:
+ Generate MD5 checksum for input files (any format)
+ Count number of sequences in fastq file
+ Verify the original checksum, and highlight any mismatch

![FqHashForWin Screenshot](https://github.com/hliang/FqHashForWin/raw/master/FqHashForWin-screenshot.PNG)

## Installation
<em>Requirement: Windows 7/8/10</em>

Download the latest version (FqHashForWin.zip) from the [release page](https://github.com/hliang/FqHashForWin/releases) to your computer, unzip, and double-click the exe file.

## Using FqHashForWin
1. Add files/folders by one of the ways below:
    + Simply drag and drop your files/folders in the window.
    + If a folder is chosen, all sequence files inside will be added to the table.
2. (Optional) Check "Count Sequences" if needed.
3. Click "Run" to start processing.
4. If you have the original checksum, enter it in the "Verify Checksum" column to verify.

Note: if `Total Seq` = -1, that means there is an error processing the sequence file, probably because the file is corrupted for malformatted.
