package com.sthService.config;

import com.sthService.repository.ModelChangeRepository;
import com.sthService.service.ModelChangeService;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class ServiceConfiguration {

    @Bean
    ModelChangeService modelChangeService() {
        return new ModelChangeService();
    }
}
