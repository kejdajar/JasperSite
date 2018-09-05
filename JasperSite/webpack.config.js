

const path = require('path');
module.exports = {
mode: "production",
    entry: {
        theme: "./wwwroot/scripts/theme.js",
        admin: "./wwwroot/scripts/admin.js",
        login: "./wwwroot/scripts/login.js",
        chart: "./wwwroot/scripts/chart.js",
        
     
    },
    output: {
path: path.resolve(__dirname,"wwwroot/bundles"),
filename: "[name].bundle.js"

    },
    module:{
        rules:[
            {
                test:/\.css$/,
                use:['style-loader','css-loader']
            },

            /* regex for font-awesome */
            { test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: "url-loader" },
            { test: /\.(ttf|eot|svg)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: "url-loader" },

            /* regex for images in css file */
            {
                test: /\.(png|jpg)$/, loader: 'url-loader'
            }
       ]
    }

};

