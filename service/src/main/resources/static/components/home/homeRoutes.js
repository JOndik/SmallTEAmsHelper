/**
 * Created by Jakub Ondik on 14.9.2015.
 */
'use strict';

angular
    .module('app.home')
    .config(homeRoutes);

function homeRoutes($stateProvider) {
    $stateProvider.state('home', {
        url: '/',
        templateUrl: 'components/home/homeView.html',
        controller: 'homeController',
        controllerAs: 'vm'
    });
}