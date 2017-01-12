#!/usr/bin/env groovy

node {
  stage('compile') {
    checkout scm
    stash 'everything'
    dir('src/cafe') {
      bat 'dotnet restore'
      bat "dotnet build --version-suffix ${env.BUILD_NUMBER}"
    }
  }
}

// stage('test') {
//     parallel unitTests: {
//       test('Test')
//     }, integrationTests: {
//       test('IntegrationTest')
//     },
//     failFast: false
// }

// def test(type) {
//   node {
//     unstash 'everything'
//     dir("test/cafe.${type}") {
//         bat 'dotnet restore'
//         bat 'dotnet test'
//     }
//   }
// }

stage('publish') {
  parallel windows: {
    publish('win10-x64', 'windows')
  }, centos: {
    publish('centos.7-x64', 'centos')
  }, ubuntu: {
    publish('ubuntu.16.04-x64', 'ubuntu')
  }
}

def publish(target, imageLabel) {
  node {
    unstash 'everything'
    dir('src/cafe') {
      bat 'dotnet restore'
      bat "dotnet publish -r ${target}"
      archiveArtifacts "bin/Debug/netcoreapp1.1/${target}/publish/*.*"
      bat "docker build -f Dockerfile-${imageLabel} -t cafe:${imageLabel} ."
      withCredentials([[$class: 'UsernamePasswordMultiBinding', credentialsId: 'DockerHubCredentials',
        usernameVariable: 'USERNAME', passwordVariable: 'PASSWORD']]){
          echo "Logged into DockerHub with username ${env.USERNAME}"
          def user = env.USERNAME
          def password = env.PASSWORD
          bat "docker login -u ${user} -p ${password}"
        }
      }
  }  
}

// docker run -d --name ${target} microsoft/windowsservercore
// docker run -d --name ${target} centos
// docker run -d --name ${target} ubuntu:xenial

// docker cp ./bin/Debug/netcoreapp1.1/${target}/publish/. ${target}:/usr/share/cafe

// docker commit ${target} cafe:windows
// docker commit ${target} cafe:centos
// docker commit ${target} cafe:ubuntu

// docker stop CONTAINER ${target}

// Where is the image file at? 
// Usage:  docker import [OPTIONS] file|URL|- [REPOSITORY[:TAG]]
// 
// Import the contents from a tarball to create a filesystem image
// 
// Options:
//   -c, --change value     Apply Dockerfile instruction to the created image (default [])
//       --help             Print usage
//   -m, --message string   Set commit message for imported image