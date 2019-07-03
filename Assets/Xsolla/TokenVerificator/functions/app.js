// Copyright 2019 Xsolla Inc. All Rights Reserved.
// @author Vladimir Alyamkin <ufna@ufna.ru>

const config = require('./config.json');
global.gConfig = config;

let express = require('express');
let logger = require('morgan');

let tokenRouter = require('./routes/token');

let app = express();

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));

app.use('', tokenRouter);

module.exports = app;
