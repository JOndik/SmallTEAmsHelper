package com.sthService.service;

import com.sthService.dataContract.SmallTeam;
import com.sthService.repository.SmallTeamRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import java.util.ArrayList;

@Service
@Transactional
public class SmallTeamService {

    @Inject
    private SmallTeamRepository smallTeamRepository;

    public SmallTeam getByUserId(String id) {
        return smallTeamRepository.findByNestedUserId(id);
    }

    public void updateTeam(SmallTeam team) {
        smallTeamRepository.save(team);
    }

    public void createSmallTeam(String id) {
        SmallTeam smallTeam = new SmallTeam();
        smallTeam.setTeamMembersId(new ArrayList<>());
        smallTeam.getTeamMembersId().add(id);
        smallTeam.setCorrespondenceModel(false);
        smallTeam.setSynchronizationAllowed(false);
        smallTeamRepository.save(smallTeam);
    }

    public void deleteSmallTeam(SmallTeam smallTeam){
        smallTeamRepository.delete(smallTeam);
    }
}
