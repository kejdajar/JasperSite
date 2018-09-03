//const path = require('path');
//module.exports = {
//mode: "production",
//    entry: "./wwwroot/scripts/main.js",
//    output: {
//path: path.resolve(__dirname,"wwwroot/bundles"),
//filename: "bundle.js"

//    },
//    module:{
//        rules:[
//            {
//                test:/\.css$/,
//                use:['style-loader','css-loader']
//            }
//       ]
//    }

//};


const path = require('path');
module.exports = {
mode: "production",
    entry: { theme: "./wwwroot/scripts/theme.js", admin:"./wwwroot/scripts/admin.js" },
    output: {
path: path.resolve(__dirname,"wwwroot/bundles"),
filename: "[name].bundle.js"

    },
    module:{
        rules:[
            {
                test:/\.css$/,
                use:['style-loader','css-loader']
            }, /* loaders for font-awesome */
            { test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: "url-loader" },
            { test: /\.(ttf|eot|svg)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: "url-loader" },
       ]
    }

};

