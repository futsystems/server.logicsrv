#!/bin/bash

version=$(git describe)

build_file="package/logicsrv/build.md"

echo $version > $build_file

