package com.sthService.service;

import com.sthService.dataContract.TeamPairRequest;
import com.sthService.repository.PairRequestRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;

@Service
@Transactional
public class PairRequestService {

    private final Logger log = LoggerFactory.getLogger(PairRequestService.class);

    @Inject
    PairRequestRepository pairRequestRepository;

    @Inject
    AISMailService aisMailService;

    /**
     * method saves pair request
     * @param request pair request
     */
    public void savePairRequest(TeamPairRequest request) {
        log.info("saving request\n");
        pairRequestRepository.save(request);
        log.info("sending email\n");
        aisMailService.sendPairRequestEmail(request.getMemberName(), request.getToken());
    }

    /**
     * method finds pair request by pair request token
     * @param pairToken pair request token
     * @return pair request
     */
    public TeamPairRequest getPairRequest(String pairToken) {
        return pairRequestRepository.findByToken(pairToken);
    }

    /**
     * method finds pair request by member name
     * @param memberName name of team member
     * @return pair request
     */
    public TeamPairRequest getPairRequestByMemberName(String memberName) {
        return pairRequestRepository.findByMemberName(memberName);
    }

    /**
     * method remove pair request
     * @param request pair request
     */
    public void deleteRequest(TeamPairRequest request) {
        pairRequestRepository.delete(request);
    }
}
