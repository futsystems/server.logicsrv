#!/bin/bash

version=$(git describe)

build_file="package/logicsrv/build.md"

echo $version > $build_file

echo -e "\033[32;49;1m LogicSrv Package Build Success Version:$version \033[39;49;0m"
