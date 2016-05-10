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

    /**
     * method finds small team by ID of its member
     * @param id ID of user
     * @return found small team
     */
    public SmallTeam getByUserId(String id) {
        return smallTeamRepository.findByNestedUserId(id);
    }

    /**
     * method updates small team
     * @param team small team to be updated
     */
    public void updateTeam(SmallTeam team) {
        smallTeamRepository.save(team);
    }

    /**
     * method creates new small team
     * @param id ID of first team member
     */
    public void createSmallTeam(String id) {
        SmallTeam smallTeam = new SmallTeam();
        smallTeam.setTeamMembersId(new ArrayList<>());
        smallTeam.getTeamMembersId().add(id);
        smallTeam.setCorrespondenceModel(false);
        smallTeam.setSynchronizationAllowed(false);
        smallTeamRepository.save(smallTeam);
    }

    /**
     * method removes small team
     * @param smallTeam small team to be removed
     */
    public void deleteSmallTeam(SmallTeam smallTeam){
        smallTeamRepository.delete(smallTeam);
    }
}
