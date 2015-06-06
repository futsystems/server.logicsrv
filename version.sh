#!/bin/bash

version=$(git describe)


major_v=$(echo $version|cut -d'-' -f1|cut -d'.' -f1)
major=${major_v:1}

minor=$(echo $version|cut -d'-' -f1|cut -d'.' -f2)

fix=$(echo $version|cut -d'-' -f1|cut -d'.' -f3)

commit_num=$(echo $version| cut -d'-' -f2)
commit_hash=$(echo $version| cut -d'-' -f3)

version_file="Shared/VersionInfo.cs"
build_file="package/logicsrv/build.md"

mv $version_file $version_file.bak


echo "$major.$minor.$fix.$commit_num ${commit_hash}" > $build_file
echo "using System.Reflection;" > $version_file
echo "" >> $version_file
echo "[assembly: AssemblyVersion(\"$major.$minor.$fix.$commit_num\")]" >> $version_file
echo "[assembly: AssemblyFileVersion(\"$major.$minor.$fix.$commit_num\")]" >> $version_file
echo "[assembly: AssemblyInformationalVersion(\"$major.$minor.$fix.$commit_num ${commit_hash:1}\")]" >> $version_file
echo "[assembly: AssemblyConfiguration(\"\")]" >> $version_file


