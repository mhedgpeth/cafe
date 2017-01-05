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

stage('test') {
    parallel unitTests: {
      test('Test')
    }, integrationTests: {
      test('IntegrationTest')
    },
    failFast: false
}

def test(type) {
  node {
    unstash 'everything'
    dir("test/cafe.${type}") {
        bat 'dotnet restore'
        bat 'dotnet test'
    }
  }
}

stage('publish') {
  parallel windows: {
    publish('win10-x64')
  }, centos: {
    publish('centos.7-x64')
  }, ubuntu: {
    publish('ubuntu.16.04-x64')
  }
}

def publish(target) {
  node {
    unstash 'everything'
    dir('src/cafe') {
      bat "dotnet publish -r ${target}"
      archiveArtifacts "bin/Debug/netcoreapp1.1/${target}/publish/*.*"
    }
  }
}