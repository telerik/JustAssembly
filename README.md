#JustAssembly

JustAssembly is a lightweight .NET assembly diff and analysis tool built on top of the [Telerik JustDecompile Engine](https://github.com/telerik/JustDecompileEngine). As opposed to just comparing signatures, it produces a diff on all assembly contents including the code of the methods.

Copyright (c) 2011 - 2016 Telerik AD

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

##Download Binaries

You can download the latest binary version from the [Telerik website](http://www.telerik.com/justassembly).

##How to Contribute

###Before starting work on a feature or a fix, please:

* Add a new issue in the [Issues](https://github.com/telerik/Medusa/issues) section or find an existing issue describing the problem.
* Let us know thru the issue comments that you are working on a particular issue so that we can change its status to "In Progress," alert the rest of the community and avoid duplicating efforts.
* If choosing between more than one issues, please, start working on the one with the higher priority.

###Before submitting a pull request:

Read and sign the [Contributors License Agreement](https://docs.google.com/a/telerik.com/forms/d/1NHkl8cWYOU4kwiyBZi0o745mMbNk7Y-esYmBgCEMakM/viewform)

###Merging pull requests

We'll do our best to merge pull requests within a day or two, especially if they are major. Once the merge is done, it takes 1-3 days to create a new binary and post it on the [Telerik website](http://www.telerik.com/justassembly).

## How to Get Started

JustAssembly uses [JustDecompileEngine](https://github.com/telerik/JustDecompileEngine) as a submodule. In order to get the submodule's code together with the main repo you need to pass `--recursive` to the `git clone` command. If you already cloned the repo in the traditional way, don't worry. First you need to initialize your local configuration file using `git submodule init`. Then use `git submodule update` to fetch all the data from [JustDecompileEngine](https://github.com/telerik/JustDecompileEngine) and check out the appropriate commit.

## Roadmap for JustAssembly

For roadmap and milestones, check the [Issues](https://github.com/telerik/Medusa/issues) section.

## Feedback and Suggestions about JustAssembly

If you find a bug or want to suggest a feature, please use the [Issues](https://github.com/telerik/Medusa/issues) section.

For feedback and discussions, please visit the [Telerik Forums](http://www.telerik.com/forums/justassembly).
