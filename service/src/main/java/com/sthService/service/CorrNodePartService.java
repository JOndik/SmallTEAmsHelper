package com.sthService.service;

import com.sthService.dataContract.CorrespondenceNodePart;
import com.sthService.repository.CorrNodePartRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import java.util.List;

@Service
@Transactional
public class CorrNodePartService {

    @Inject
    private CorrNodePartRepository corrNodePartRepository;

    public String createCorrNodePart(String userName, String elementGUID, String modelGUID, String smallTeamId){
        CorrespondenceNodePart correspondenceNodePart = new CorrespondenceNodePart();
        correspondenceNodePart.setUserName(userName);
        correspondenceNodePart.setElementGUID(elementGUID);
        correspondenceNodePart.setModelGUID(modelGUID);
        correspondenceNodePart.setSmallTeamID(smallTeamId);
        corrNodePartRepository.save(correspondenceNodePart);
        return correspondenceNodePart.getId();
    }

    public List<CorrespondenceNodePart> getCorrNodePartsBySmallTeamId(String id){
        return corrNodePartRepository.findBySmallTeamID(id);
    }

    public CorrespondenceNodePart getCorrNodePart(String userName, String itemGUID, String modelGUID, String smallTeamId){
        return corrNodePartRepository.findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(userName, itemGUID, modelGUID, smallTeamId);
    }

    public CorrespondenceNodePart getCorrNodePartById(String id){
        return corrNodePartRepository.findById(id);
    }

    public void deleteCorrNodePart(CorrespondenceNodePart correspondenceNodePart){
        corrNodePartRepository.delete(correspondenceNodePart);
    }
}
