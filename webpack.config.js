var path = require('path');
var fs = require('fs');

function resolve(filePath) {
  return path.resolve(__dirname, filePath)
}

var babelOptions = {
  "presets": [
    [resolve("./node_modules/babel-preset-es2015"), {"modules": false}]
  ]
}

module.exports = {
  entry: resolve('./tests/Fable.Aether.Tests.fsproj'),
  output: {
    filename: 'tests.bundle.js',
    path: resolve('./build'),
  },
  target: "node",
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: "fable-loader",
          options: { 
            fableCore: resolve("./packages/Fable.Core/fable-core"),
            plugins: resolve("./node_modules/fable-plugins-nunit/Fable.Plugins.NUnit.dll"),
            babel: babelOptions 
          }
        }
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: babelOptions
        },
      }
    ]
  },
};