import hashlib
import unittest
from pathlib import Path
import sys
import numpy as np
sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
import rig_ch1_doeon_left_prototype as left
import rig_ch1_doeon_left_combat as combat

class CombatCandidateTests(unittest.TestCase):
    @staticmethod
    def synthetic_actor():
        frame=np.zeros((256,256,4),dtype=np.uint8)
        frame[57:95,118:141,:]=[160,135,98,255]
        frame[95:155,113:147,:]=[35,44,60,255]
        frame[94:152,100:113,:]=[77,108,147,255]
        frame[94:152,149:163,:]=[77,105,140,255]
        frame[150:205,112:129,:]=[55,59,68,255]
        frame[151:205,134:155,:]=[51,59,74,255]
        frame[199:209,97:131,:]=[108,76,40,255]
        frame[199:209,134:164,:]=[121,80,35,255]
        return frame
    def test_24_distinct_bounded_articulated_frames(self):
        parts,_=left.layers(self.synthetic_actor())
        for row in combat.ANIMATION_ROWS:
            unique=set()
            for i in range(6):
                im=np.asarray(combat.compose_action(parts,row,i))
                self.assertEqual(im.shape,(256,256,4))
                self.assertEqual(int(np.count_nonzero(im[0,:,3])),0)
                ys,xs=np.nonzero(im[:,:,3]>30)
                self.assertGreater(len(xs),700)
                self.assertEqual(int(ys.max()),left.RAW_FOOT_Y)
                self.assertLess(int(xs.max()),220)
                self.assertGreater(int(xs.min()),15)
                unique.add(hashlib.sha256(im.tobytes()).hexdigest())
            self.assertGreaterEqual(len(unique),5,row)
    def test_attack_is_left_hand_reach_not_mirrored(self):
        a=combat.ATTACK['basic_attack']
        self.assertLess(a['front'][3],-60)
        self.assertGreater(a['body'][3],0)
        self.assertEqual(left.LEFT_SHOULDER,(109,99))
        self.assertEqual(combat.ANIMATION_ROWS[0],'basic_attack')
    def test_recoil_is_proportional_and_resettable(self):
        h=combat.HURT['hit_recoil']
        self.assertLess(h['body'][3],h['body'][1])
        self.assertEqual(h['body'][-1],0)
        self.assertEqual(h['shift'][-1],0)
        self.assertEqual(h['front'][-1],0)
    def test_experimental_rows_are_not_claimed_as_named_skills(self):
        self.assertEqual(len(combat.ANIMATION_ROWS),4)
        self.assertEqual(len(set(combat.ANIMATION_ROWS)),4)
        self.assertTrue(all(x.endswith('_unbound') for x in combat.ANIMATION_ROWS[2:]))

if __name__=='__main__':
    unittest.main()