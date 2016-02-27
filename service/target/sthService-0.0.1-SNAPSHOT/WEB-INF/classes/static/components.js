/**
 * Created by Jakub Ondik on 27.8.2015.
 */
'use strict';

/**
 * Main app - app.js
 */
angular
    .module('app', [
        'ngCookies',
        'ngResource',
        'ngCookies',
        'ngMaterial',
        'ngAnimate',
        'ui.router',
        'app.constants',
        'app.home',
        'app.change']);

/**
 * Global constants - constants.js
 */
angular
    .module('app.constants', []);

/**
 * Home page - components/home/
 */
angular
    .module('app.home', [
        'ui.router',
        'app.change']);

/**
 * Change resource - components/change/
 */
angular
    .module('app.change', [
        'app.constants']);
