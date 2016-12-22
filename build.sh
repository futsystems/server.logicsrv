#!/bin/bash

version=$(git describe)

build_prog=$1

echo "Generate build file of ${build_prog}"

build_file="build/${build_prog}/bin/build.md"

echo $version > $build_file

