# JustAssembly

JustAssembly is a lightweight .NET assembly diff and analysis tool built on top of the [Telerik JustDecompile Engine](https://github.com/telerik/JustDecompileEngine). As opposed to just comparing signatures, it produces a diff on all assembly contents including the code of the methods.

![alt text](https://d585tldpucybw.cloudfront.net/sfimages/default-source/productsimages/justassembly/how-it-works.png)

## Command line usage

It is possible to use JustAssembly with command line interface, in order to generate an XML file with assembly public API differences.

That effectively makes it possible to use JustAssembly in an automated build pipeline to analyse differences between the output of the previous and the latest build (Semantic Versioning in future release, see issue #14 for details).

*Commandlinetool.exe* accepts three arguments
1) First assembly path
2) Second assembly path
3) XML output path 

```
justassembly.commandlinetool.exe Path\To\Assembly1 Path\To\Assembly2 Path\To\XMLOutput.xml
```

## License

Copyright (c) 2011 - 2018 Telerik EAD

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

## Download Binaries

You can download the latest binary version from the [Telerik website](http://www.telerik.com/justassembly).

## How to Contribute

### Before starting work on a feature or a fix, please:

* Add a new issue in the [Issues](https://github.com/telerik/JustAssembly/issues) section or find an existing issue describing the problem.
* Let us know thru the issue comments that you are working on a particular issue so that we can change its status to "In Progress," alert the rest of the community and avoid duplicating efforts.
* If choosing between more than one issues, please, start working on the one with the higher priority.

### Before submitting a pull request:

Read and sign the [Contributors License Agreement](https://docs.google.com/forms/d/e/1FAIpQLSc3w8R1GemVK66EI9mXjTet7izmBf1ziPhe4P9CYZN6ZA4Yhg/viewform)

### Merging pull requests

We'll do our best to merge pull requests within a day or two, especially if they are major. Once the merge is done, it takes 1-3 days to create a new binary and post it on the [Telerik website](http://www.telerik.com/justassembly).

## How to Get Started

JustAssembly uses [JustDecompileEngine](https://github.com/telerik/JustDecompileEngine) as a submodule. In order to get the submodule's code together with the main repo you need to pass `--recursive` to the `git clone` command. If you already cloned the repo in the traditional way, don't worry. First you need to initialize your local configuration file using `git submodule init`. Then use `git submodule update` to fetch all the data from [JustDecompileEngine](https://github.com/telerik/JustDecompileEngine) and check out the appropriate commit.

## Roadmap for JustAssembly

For roadmap and milestones, check the [Issues](https://github.com/telerik/JustAssembly/issues) section.

## Feedback and Suggestions about JustAssembly

If you find a bug, want to suggest a feature or discuss existing ones, please use the [Issues](https://github.com/telerik/JustAssembly/issues) section.
