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

    /**
     * method creates correspondence node part
     * @param userName name of user
     * @param elementGUID GUID of item
     * @param modelGUID GUID of model
     * @param smallTeamId ID of small team
     * @return ID of new correspondence node part
     */
    public String createCorrNodePart(String userName, String elementGUID, String modelGUID, String smallTeamId){
        CorrespondenceNodePart correspondenceNodePart = new CorrespondenceNodePart();
        correspondenceNodePart.setUserName(userName);
        correspondenceNodePart.setElementGUID(elementGUID);
        correspondenceNodePart.setModelGUID(modelGUID);
        correspondenceNodePart.setSmallTeamID(smallTeamId);
        corrNodePartRepository.save(correspondenceNodePart);
        return correspondenceNodePart.getId();
    }

    /**
     * method finds all correspondence node parts owned by small team
     * @param id ID of small team
     * @return list of all found correspondence node parts
     */
    public List<CorrespondenceNodePart> getCorrNodePartsBySmallTeamId(String id){
        return corrNodePartRepository.findBySmallTeamID(id);
    }

    /**
     * method finds correspondence node parts by parameters
     * @param userName name of user
     * @param itemGUID GUID of item
     * @param modelGUID GUID of model
     * @param smallTeamId ID of small team
     * @return found correspondence node part
     */
    public CorrespondenceNodePart getCorrNodePart(String userName, String itemGUID, String modelGUID, String smallTeamId){
        return corrNodePartRepository.findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(userName, itemGUID, modelGUID, smallTeamId);
    }

    /**
     * method finds correspondence node part by its ID
     * @param id ID of correspondence node part
     * @return found correspondence node part
     */
    public CorrespondenceNodePart getCorrNodePartById(String id){
        return corrNodePartRepository.findById(id);
    }

    /**
     * method removes correspondence node part
     * @param correspondenceNodePart correspondence node part
     */
    public void deleteCorrNodePart(CorrespondenceNodePart correspondenceNodePart){
        corrNodePartRepository.delete(correspondenceNodePart);
    }
}
