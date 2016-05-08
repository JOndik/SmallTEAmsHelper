/**
 * Created by Jakub Ondik on 14.9.2015.
 */
'use strict';

angular
    .module('app.home')
    .controller('homeController', homeController);

function homeController($rootScope, $state, Change) {
    var vm = this;
    vm.changeSet = Change.query();
}