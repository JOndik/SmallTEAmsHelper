package com.sthService.config;

import com.sthService.service.AuthorizationService;
import com.sthService.service.ModelChangeService;
import com.sthService.service.UpdateService;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class ServiceConfiguration {

    @Bean
    ModelChangeService modelChangeService() {
        return new ModelChangeService();
    }

    @Bean
    AuthorizationService logInService() {
        return new AuthorizationService();
    }

    @Bean
    UpdateService updateService() {
        return new UpdateService();
    }
}
