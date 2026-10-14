# NEEDLIT — แผนพัฒนาเต็มจนถึง Steam

เวอร์ชัน 1.0 — 14 ตุลาคม 2026

แบบเกมหลัก: [GAME_DESIGN.md](GAME_DESIGN.md) • การตรวจรับ: [CHECKLIST.md](CHECKLIST.md)

เอกสารนี้เป็นแผนล่วงหน้า ไม่ได้หมายความว่าสร้าง scene, asset, executable หรือผ่าน test แล้ว

## 1. สภาพแวดล้อมที่ตรวจพบ

| รายการ | ผลตรวจวันที่จัดทำ |
|---|---|
| Workspace | `D:\Code\needlit` เริ่มต้นเป็นโฟลเดอร์ว่าง |
| Unity executable | `C:\Program Files\Unity\Hub\Editor\6000.3.26f1\Editor\Unity.exe` |
| Product version ที่อ่านจากไฟล์ | `6000.3.26f1_de8167e7440c` |
| Build support ที่พบ | `windowsstandalonesupport` |
| โปรเจกต์/โค้ดเดิม | ไม่พบ `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json` หรือโค้ดเกม |
| คำสั่งเฉพาะ repo | ไม่พบ `AGENTS.md` ใน workspace หรือไฟล์ที่ตรวจใน parent ที่เกี่ยวข้อง |
| ยังไม่ได้ตรวจ | Unity เปิดและ activate ได้หรือไม่, package cache/network, compiler, GPU/CPU/RAM, Steamworks account, สิทธิ์ asset |

เริ่มจาก Unity ที่มีอยู่ ไม่เปลี่ยนเวอร์ชันเองระหว่างทาง การเปิด Editor จริง สร้างโปรเจกต์ และทดลอง build เป็นงาน M0

## 2. วิธีทำงานตามที่ผู้ใช้กำหนด

**โมเดลหลักปัจจุบันเป็นผู้ออกแบบงานและ reviewer; sub-agent `gpt-6-sol` เป็นผู้เขียนโค้ด** ใช้ชื่อนี้แบบตรงตัว ไม่สับสนกับ `gpt-6.1-sol` หรือ `gpt-6-astra` และไม่เปลี่ยนโมเดลโดยไม่แจ้งหากเรียกใช้ไม่ได้

รอบจัดทำเอกสารนี้ยังไม่มีงานโค้ดให้มอบหมาย จึงไม่ได้เริ่มพัฒนาเกมหรือเรียก sub-agent เขียนเกมล่วงหน้า

### วงจรต่อหนึ่งงาน

1. โมเดลหลักอ่านเอกสารล่าสุด โค้ดที่เกี่ยวข้อง และ `AGENTS.md` หากถูกเพิ่มเข้ามา ตรวจสถานะไฟล์ที่ผู้ใช้แก้เอง
2. เลือก task ID จาก checklist ตาม dependency บอกขอบเขตไฟล์และพฤติกรรมที่ต้องผ่าน รวม callers, tests, scenes/prefabs, data และ config
3. มอบงานให้ agent ด้วย `model: "gpt-6-sol"` และ `fork_turns: "none"` พร้อมสรุปบริบทที่จำเป็นครบ ไม่อาศัยว่า agent เห็นประวัติทั้งหมด
4. Agent เขียนการเปลี่ยนแปลงและ test สำหรับตรรกะสำคัญ ใช้ระบบ/เครื่องมือเดิมก่อนเพิ่ม dependency และส่งรายงานไฟล์ที่แก้ วิธีทดสอบ ผลจริง และสิ่งที่ยังไม่ตรวจ
5. โมเดลหลักอ่าน diff และเส้นทางโค้ดที่เชื่อมต่อ ตรวจ logic และ edge cases เอง ไม่รับเพียงคำว่า “tests pass”
6. โมเดลหลักรัน check ที่เกี่ยวข้องและทดลอง Editor/build ตามความเสี่ยง ถ้าพบปัญหาส่งกลับให้ `gpt-6-sol` แก้ แล้วตรวจใหม่
7. เมื่อผ่านจึงรวมงานและอัปเดต checklist พร้อมหลักฐาน ถ้ายังไม่มีเครื่องมือทดสอบจริง ให้คงสถานะ “ยังไม่ตรวจ” ไม่ติ๊กเสร็จ
8. ส่ง progress ที่มี build/ผลทดสอบชัดเจนเมื่อจบ milestone; เก็บงานให้ผู้ใช้ทยอยสั่งต่อและตรวจรวมท้ายโครงการได้

ผู้ใช้ไม่ต้องมานั่งตรวจโค้ดทุก task แต่การทดสอบว่า “สนุก” และ “ไม่เมื่อยมือ” ต้องมีคนเล่นจริงก่อนขาย ไม่ใช้ผล unit test แทนความเห็นผู้เล่น

### การแบ่ง agent และไฟล์

- ค่าเริ่มต้น agent เขียนโค้ดหนึ่งตัวต่อหนึ่งงานที่เชื่อมกัน; แยกสองงานพร้อมกันเฉพาะเมื่อ contracts ชัดและไม่แก้ไฟล์ร่วม
- ระบบนี้มีเพดานรวม 4 agent พร้อมกันรวมโมเดลหลัก จึงใช้ลูกได้ไม่เกิน 3 ตัว; ไม่จำเป็นต้องใช้เต็ม
- ไม่ให้หลาย agent เขียน `.unity`, `.prefab`, `ProjectSettings` หรือ `Packages/manifest.json` เดียวกันพร้อมกัน
- งาน C# แบ่งตาม folder ได้ แต่ Unity import/build/test ใช้ลำดับเดียวกับ project ที่เปิดอยู่ ป้องกัน asset database ชนกัน
- จัดเจ้าของ scene/prefab ชัด และห้าม regenerate scene เพื่อทับงานศิลป์ที่มีคนแก้แล้ว
- บันทึกผลใน `docs/PROGRESS.md` เมื่อเริ่ม M0 พร้อม task ID, build version, files, test results, known issues และ next task

### ตัวอย่างใบงานให้ agent

```text
Task: M1-02 ฝนเฉพาะด้าน
Model: gpt-6-sol
อ่าน: GAME_DESIGN.md §5, DEVELOPMENT_PLAN.md §3–4, checklist งาน M1
ขอบเขต: Grinding/StockShape.cs, Grinding/GrindingStroke.cs และ tests ที่เกี่ยวข้อง
ต้องทำ: ลดเนื้อเฉพาะพื้นที่สัมผัส, ไม่เพิ่มเนื้อ, ค่าทุกตัว finite,
        ฝั่งตรงข้ามไม่สึก, stroke เดียวให้ผลใกล้กันที่ 30/60/144 FPS
ห้าม: เพิ่มระบบเงิน, แก้ scene ที่ agent อื่นเป็นเจ้าของ, เปลี่ยน package version
ตรวจรับ: deterministic tests และรายงานผลจริง; ระบุสิ่งที่ยังไม่รัน
ส่งกลับ: ไฟล์ที่แก้, วิธีตรวจ, ข้อจำกัด, ประเด็นที่ reviewer ควรดู
```

## 3. แนวทางทางเทคนิคที่เลือก

### โปรเจกต์อ้างอิงในเครื่อง: CCC Simulator

ผู้ใช้ให้ดู `D:\Code\ccc-simulator` เมื่อ 14 ตุลาคม 2026 ตรวจแบบอ่านอย่างเดียว พบ Unity รุ่นเดียวกัน และไฟล์ตั้งค่าจริงใช้ URP 17.3.0 กับ Input System แม้ README บางส่วนยังระบุเทคโนโลยีเก่า

- โครงสร้างแยก `Unity/Assets/_Game`, `docs`, `tools` และ `Builds`; NEEDLIT คง layout ที่เริ่มไว้ ไม่ย้ายโฟลเดอร์เพียงเพื่อให้เหมือนกัน
- `Scripts/Editor/BuildTools.cs` และ `tools/build.sh` เป็นตัวอย่าง build entry point ที่ตรวจผลสำเร็จและ exit code
- `Scripts/Editor/SmokeTestRunner.cs` เป็นแนวทางตรวจ scene และภาพ UI; ใช้เป็นข้อมูลสำหรับการทดสอบของ NEEDLIT เมื่อมี UI จริง
- `IInteractable.cs`, `ComputerTerminal.cs`, `DeliveryScheduler.cs` และ `SaveSystem.cs` เป็นจุดอ่านประกอบก่อนทำ M2: interaction, คอม, delivery และ save versioning ไม่คัดลอกระบบทั้งเกมมาล่วงหน้า

ยังไม่ได้รันหรือ audit CCC ทั้งโปรเจกต์ และไม่ได้แก้ไฟล์ใน repo นั้น

### โครงสร้างโปรเจกต์

วาง Unity project ที่ root นี้เพื่อใช้เอกสารร่วมกับโค้ดได้ตรง ๆ ใช้ C# และ GameObject/MonoBehaviour ทั่วไป เริ่มด้วย render pipeline ที่ตั้งใจใช้ขายคือ URP หาก template/package ที่รองรับเวอร์ชันที่ติดตั้งใช้งานได้ ยืนยัน package จริงใน M0 และ commit lock file ไม่เดาเลขเวอร์ชันจากความจำ

```text
Assets/Needlit/
  Scenes/             Bootstrap, Home, Mountain, GrindingLab
  Scripts/
    Player/           movement, interaction, input modes
    Grinding/         shape, stroke, tools, evaluation, presentation
    Porter/           route, balance, cargo, contract, payout
    Economy/          wallet, catalog, order, delivery
    Casino/           shared round state + rules of 7 games
    Persistence/      save data, validation, migration, atomic write
    UI/               menus, computer, HUD, tutorials, localization
    Progression/      milestones, ending, achievements
  Data/               item definitions, needle targets, route patterns
  Art/                meshes, materials, animations, textures
  Audio/
  Prefabs/
  Tests/EditMode/
  Tests/PlayMode/
docs/                 progress, decisions, test reports, licenses
Builds/               local artifacts; ไม่เก็บ binary ใน source control
```

ใช้ ScriptableObject สำหรับข้อมูลเครื่องมือ/แม่แบบ/สัญญาที่ปรับบาลานซ์ และ plain C# สำหรับกติกาที่ test แยกได้ ใช้ event หรือการเรียกตรงเท่าที่จำเป็น ไม่สร้าง global event bus, dependency injection framework, ECS หรือ network backend

ใช้ Input System สำหรับแยกโหมดเดิน/โต๊ะฝน/คอม/บ่อน และ UI ที่ทีมเลือกใน M0 เพียงระบบเดียว ค่าเริ่มต้น uGUI + TextMeshPro เพราะต้องทำจอคอมและ HUD พร้อมฟอนต์ไทย ตรวจ font license และ fallback จริงก่อนเลือกชุดฟอนต์

ระบบเซฟใน local disk ต้องใช้ได้โดยไม่มี Steam ส่วน Steam integration ใส่ท้ายโครงการและเลือก wrapper C# ที่มีสิทธิ์ใช้และเข้ากับเวอร์ชันจริงในตอนนั้น ไม่เพิ่ม dependency นี้ตั้งแต่ต้น

### โหมดเกม

`Explore`, `Grinding`, `Computer`, `Porter`, `Casino`, `Paused` มีเจ้าของ input/cursor เพียงโหมดเดียวในแต่ละขณะ เมื่อเข้า UI ให้เครื่องฝนและการเดินหยุดตามกติกา และเมื่อ application เสีย focus ให้หยุด/ล้าง mouse delta ก่อนกลับ

Bootstrap โหลด settings/save และเปลี่ยน scene; Home รวมบ้านและบ่อนซึ่งอยู่ใกล้กัน; Mountain โหลดเมื่อเริ่มสัญญา; GrindingLab เป็นฉากพัฒนาไม่รวมใน build ขาย

## 4. ตัวแทนรูปทรง: ตัดสินใจให้ต้นแบบพิสูจน์ได้

Unity รองรับการสร้างและเปลี่ยน mesh จากสคริปต์ แต่ไม่ได้ทำให้ระบบกัดเนื้อ/ตรวจแม่แบบสำเร็จรูป เราต้องเขียนและทดสอบส่วนนี้เอง ดู [Unity Manual: Creating meshes](https://docs.unity3d.com/6000.3/Documentation/Manual/creating-meshes.html)

### 4.1 วิธีแรกที่ให้ทดลอง

ใช้ตารางรัศมี `r[z, angle]` รอบแกนแม่แบบร่วมกับขอบเขตปลายตามแกน เก็บแต่ละมุมแยกกัน จึงทำให้ฝั่งหนึ่งแบนหรือบางกว่าอีกฝั่งได้ ต่างจาก lathe profile ที่มีรัศมีเดียวต่อวงและจะทำให้ทุกด้านสึกพร้อมกัน

- เริ่มประมาณ 256 sample ตามยาว ×64 มุม ประมาณ 16k surface samples แล้ววัดผลจริง
- หน้าตัดเป็นรูปที่แต่ละรังสีจากแกนตัดผิวนอกหนึ่งครั้ง ไม่รองรับโพรงภายในหรือเศษที่แยกเป็นหลายชิ้น
- การฝนหน้าปลายใช้การเลื่อน end boundary เฉพาะตามกติกาปากกาจับ/เครื่องฝนหน้าปลาย ไม่แกล้งย่อทั้งวัตถุ; M1 ต้องทำได้ก่อนถือว่าต้นแบบสำเร็จ
- มีตัวจับงานให้เลือกด้านข้าง/หัว/ท้าย เพื่อไม่ให้ข้อจำกัดหน้าปลายเป็นช่องว่างที่ผู้เล่นเข้าถึงไม่ได้
- ส่วนที่ไม่อยู่ในรูปแบบนี้ เช่น เจาะห่วง ใช้ local mesh patch ตามแบบ §5.6 ไม่ใช้ decal
- ข้อมูล shape เป็นแหล่งจริง; mesh เป็นภาพสร้างจากข้อมูล ไม่อ่าน vertices ที่เปลี่ยนเพราะ visual effect กลับไปใช้ตัดสินคะแนน

วิธีนี้เป็นข้อเสนอทางวิศวกรรมที่ยังไม่ผ่านต้นแบบ หากทำให้การฝนความยาวหรือรอยเฉพาะจุดดูผิดชัด ต้องแก้ตัวแทนข้อมูลก่อนสร้างเนื้อหาขั้นถัดไป ห้ามตัดความต้องการผู้ใช้ให้พอดีกับ implementation

### 4.2 รองรับตั้งแต่เมตรถึงมิลลิเมตร

Uniform grid ยาวหนึ่งเมตรละเอียดไม่พอสำหรับเข็ม 50 มม. จึงวาง sample หนาแน่นในช่วงแกนที่แม่แบบสุดท้ายอยู่ และหยาบในส่วนเกินไกลออกไป ใช้ตำแหน่ง z ที่ไม่จำเป็นต้องห่างเท่ากัน แม่แบบท้ายเกมต้องมี sample พอสร้างปลายและหัวได้ตั้งแต่ก่อนเริ่มฝน

เมื่อส่วนเกินหาย สามารถเลิก render segment ที่ไม่มีเนื้อและปรับ framing กล้อง โดยไม่ rescale เนื้อเหล็กจริง ถ้าจำเป็นต้องเพิ่ม sample ให้ใช้ conservative resampling ที่ไม่เติมเนื้อคืนและเก็บตำหนิเดิมไว้ พร้อม test เปรียบเทียบก่อน/หลัง

ใช้ local coordinates ใกล้ origin; มุมมองโต๊ะฝนจัดกล้อง/clip plane ให้เหมาะกับสเกล ไม่ทำ collider ของโลกให้เล็กจน input เสียความแม่นยำ การ hit test ใช้รูปทรงปัจจุบันและหน้าสัมผัสเครื่องมือ ไม่ใช้ collider ทรงเสาเดิมตลอดเกม

เริ่ม CPU mesh update เฉพาะส่วนที่เปลี่ยนก่อน อย่าเริ่มด้วย voxel, GPU compute หรือ Boolean ทั่วไป เฉพาะเมื่อ profiler และข้อจำกัดรูปทรงพิสูจน์ว่าแนวแรกไม่ผ่าน จึงทำ spike ทางเลือกแบบมีงบเวลา

### 4.3 กฎที่ code ต้องรักษา

1. เนื้อเหล็กและความยาวไม่เพิ่มจากการฝน การเปลี่ยนเครื่องมือ การซูม การโหลด หรือการเปลี่ยน resolution
2. ย้าย/หมุนวัตถุแล้วรอยสึกอยู่พิกัดวัตถุเดิม ไม่เลื่อนไปตามกล้อง
3. หมุนผ่านมุม 359°/0° ไม่มีรอยต่อแตกและแปรงทำงานต่อเนื่อง
4. ผลไม่ขึ้นกับ FPS: replay เส้นทางเดียวกันที่ 30/60/144 FPS ให้ปริมาณสึกและผลตรวจใกล้กันใน tolerance ที่กำหนดไว้
5. แปรงกินเฉพาะด้านที่มองเห็นและสัมผัส ไม่ทะลุไปอีกด้าน
6. ไม่มี radius ติดลบ, NaN, Infinity, flipped triangles หรือ mesh bounds ผิดหลังแก้รูป
7. ฝนถึงแกน/รอยแตกที่ทำให้ชิ้นงานใช้ไม่ได้ต้องเปลี่ยนสถานะเป็นงานเสียอย่างชัดเจน ไม่สร้าง mesh ทะลุตัวเอง
8. แม่แบบ/ghost และตัวประเมินใช้ข้อมูลเป้าหมายเดียวกัน มีหน่วยเดียวกัน; อย่าใช้ภาพหนึ่งกับ tolerance อีกชุด
9. การประเมินรวมตำแหน่งผิดสูงสุดและบริเวณระหว่าง sample/สามเหลี่ยม ไม่ดูเพียงค่าเฉลี่ยหรือ sparse sample ที่พลาดรอยบิ่น
10. ข้อจำกัดของตัวแทนข้อมูลที่ยอมรับต้องบันทึกเป็น `shortcut: <ข้อจำกัด>, <เมื่อใดต้องเปลี่ยน>` ใกล้โค้ดที่เกี่ยวข้อง

### 4.4 แผน fallback ของความเสี่ยงหลัก

| ปัญหา | ขั้นแรก | ถ้ายังไม่ผ่าน |
|---|---|---|
| คุมปลายเข็มเล็กไม่ได้ | macro view, ปรับ mapping cursor และโหมดละเอียด | ปรับ tolerance/brush ให้ควบคุมได้ โดยยังต้องตรงโมเดลและบันทึกเหตุผล |
| ฝนหน้าปลายดูปลอม | ปรับ fixture และ end-surface representation | spike end-surface mesh เพิ่ม; ห้ามแทนด้วยปุ่มย่อความยาวอัตโนมัติ |
| รอยสึกหายเมื่อปรับ sample | หลีกเลี่ยง resample ใช้ target-dense grid | conservative resample + regression test |
| เจาะรูทำ seam แตก | ใช้หัวเข็ม mesh topology ที่มีขอบร่วม | แยกช่วงหัวเป็น template mesh ตั้งแต่เริ่ม แต่ยังใช้ค่ารูปจริง |
| อัปเดต mesh ช้า | dirty ranges, จำกัด update rendering, ลดงาน normals | ทดลอง job/compute เฉพาะที่ profiler ชี้ ไม่ย้ายทั้งเกม |
| ฝนซ้ำไม่สนุก | ปรับเสียง ความเร็ว และเครื่องมือที่เปลี่ยนวิธีเล่น | ปรับวงจร/ระยะเวลาใน M2 ก่อนสร้าง asset ครบ |

## 5. เงิน คำสั่งซื้อ และเซฟ

### ข้อมูลที่ต้องเก็บ

| กลุ่ม | ข้อมูลสำคัญ |
|---|---|
| Header | schema version, build version, save ID, timestamp, checksum ตามรูปแบบที่เลือก |
| Player | เงิน, inventory, อุปกรณ์ที่ติดตั้ง, ตำแหน่งที่ปลอดภัย, setting แยกไฟล์ |
| Workpiece | stock ID, template ID, shape samples, end bounds, roughness, hole parameters, failed/completed state |
| Progression | milestones, rewards already claimed, unlocked contracts, ending state, collection |
| Orders | order ID, item ID, cost, remaining play time, delivery/claim state |
| Casino | round ID, committed bet, deck/outcome/RNG state ที่ต้องใช้ต่อ, player decisions, settlement flag |
| Runtime recovery | กิจกรรมล่าสุดและ safe resume policy; งานลูกหาบที่ไม่จบกลับจุดเริ่มโดยไม่มีรายได้ |

เริ่มด้วย snapshot แบบมี version และ serialize ข้อมูลที่จำเป็น ไม่เซฟ scene ทั้งหมด ไม่ต้องมีฐานข้อมูลหรือบัญชีออนไลน์ รองรับหนึ่ง campaign slot กับ backup ที่หมุนเก็บ 2 ชุดและ settings แยก; New Game เตือนก่อนลบแคมเปญ

### ธุรกรรมและการกู้คืน

- Validate ราคา จำนวนเงิน item ID และสถานะก่อน mutation; wallet เป็นจำนวนเต็มช่วงที่กำหนด และใช้ checked arithmetic ในเส้นทางเสี่ยง overflow
- การซื้อ: หักเงินและสร้าง order ใน snapshot เดียว; claim: ให้ของและเปลี่ยนสถานะพร้อมกัน
- การพนัน: commit เงินเดิมพันกับสำรับ/ผลใน snapshot เดียว และ settlement เพิ่มเงินกับปิดรอบใน snapshot เดียว
- เขียน temp file → ตรวจความครบ → replace save เดิมอย่างปลอดภัยพร้อม backup; ถ้าเขียนล้มเหลวให้แจ้งและไม่รายงานว่าเซฟสำเร็จ
- Autosave หลังซื้อ รับของ รับค่าจ้าง จบการพนัน จบชิ้นงาน และเป็นระยะเมื่อฝน; แสดงเวลาที่บันทึกล่าสุด
- เซฟระหว่าง stroke ที่ boundary สม่ำเสมอ; pause/quit ขอ flush และแจ้งเมื่อไม่ได้บันทึก อย่าพึ่ง OnApplicationQuit อย่างเดียว
- โหลดแล้ว validate version, shape size, IDs, ranges, finite values; ไฟล์หลักเสียเสนอ backup โดยไม่เขียนทับหลักฐานทันที
- เวอร์ชันใหม่กว่าเกมให้แจ้งว่าไม่รองรับและไม่เขียนทับ; migration ต้องมี fixture ของเวอร์ชันเก่าจริงเมื่อเริ่มเปลี่ยน schema
- กัน transaction ซ้ำในเกม ไม่อ้างว่ากันผู้ใช้แก้เซฟหรือย้อน snapshot เองได้

## 6. แผน milestone และ dependency

ลำดับหลัก: **M0 → M1 → M2 → M3 → M4 → M5 → M6 → M7 → M8**

M3 กับงาน art บางส่วนทำคู่กันได้เมื่อ M2 ผ่าน; กติกาบ่อนแบบ pure C# เริ่มคู่กับ M3 ได้หลัง wallet/save contract คงที่ แต่ integration ต้องตามลำดับ ห้ามรีบสร้างสล็อตเพราะง่ายกว่าระบบฝนแล้วปล่อยความเสี่ยงหลักค้าง

หน่วยประมาณการด้านล่างเป็น **วันทำงานที่มีการตรวจและทดสอบจริง** ไม่ใช่จำนวนวันที่ AI รับประกันจะเสร็จ งานภาพ เสียง playtest และแก้ความรู้สึกไม่ลดเหลือศูนย์เพราะใช้ agent

| Milestone | ประมาณการเริ่มต้น | สิ่งที่ส่งมอบ | ด่านผ่านก่อนทำต่อ |
|---|---:|---|---|
| M0 ตั้งโปรเจกต์ | 2–3 วัน | Unity project, reproducible Windows build, test runner, repo hygiene | เปิดใหม่และ build ได้ ไม่มี missing script/package |
| M1 พิสูจน์ระบบฝน | 6–10 วัน | GrindingLab ที่ทำเสาให้เป็นเข็มได้ทั้งด้านข้าง ความยาว และห่วง | local wear, overshave, scale, save shape ผ่าน; คนทดลองควบคุมช่วงละเอียดได้ |
| M2 เล่นวงจรจริงหนึ่งรอบ | 5–8 วัน | บ้าน graybox, ลูกหาบ, คอม, delivery, 3 tools, save/load | หาเงิน→ซื้อ→ฝน→พลาด→ซื้อใหม่→ทำเข็มได้โดยไม่มี dev command |
| M3 ระบบหลักครบ | 6–10 วัน | เศรษฐกิจ/สัญญา/เครื่องมือ 16 รายการ, progression และ ending | เล่นจบโดยไม่พนันได้ และเครื่องมือเปลี่ยนวิธีเล่นจริง |
| M4 บ่อนครบ | 6–10 วัน | 7 เกมพร้อม UI, poker NPC, settlement/resume | กติกา/ความน่าจะเป็น/เงินไม่ซ้ำผ่านและกลับเข้าเกมกลางรอบได้ |
| M5 ภาพ เสียง UX และเนื้อหา | 8–14 วัน | Art pass, audio, ไทย/อังกฤษ, accessibility, แม่แบบหลังจบ | ทุกจุดหลักไม่มี placeholder สำคัญและอ่านเล่นได้ที่ 720p |
| M6 Alpha/Beta และปรับบาลานซ์ | 7–12 วัน | Full campaign builds, feedback, profiling, compatibility | ไม่มี blocker; ประสบการณ์เริ่มใหม่/ระยะเวลา/ความเมื่อยผ่าน playtest |
| M7 Steam และ release candidate | 4–7 วัน | Steam build/metadata, cloud, achievements, store package | clean-machine install, offline, overlay/cloud ตรวจแล้วตาม features ที่ประกาศ |
| M8 ตรวจรับและปล่อยจริง | 3–5 วัน | RC ที่ผู้ใช้ตรวจ, release checklist, rollback build | ผู้ใช้ตรวจงานครบและอนุมัติการเผยแพร่ build ที่ระบุ |

รวมงานประมาณ 47–79 วันทำงานก่อน contingency; กันเวลาแก้ปัญหาอีก 25–35% โดยเฉพาะ M1/M6 ไม่รวมเวลารอผู้ทดสอบ ภายนอก Steam หรือทำ asset เพิ่ม ประเมินใหม่หลัง M1 และ M2 ห้ามถือเป็นสัญญาวันขาย

### M0 — ตั้งฐานที่ทดสอบได้

สร้าง Unity project ด้วยเวอร์ชันที่ตรวจพบ ทดสอบ license/package import และ Windows build ก่อนสร้างระบบเกม เก็บ `.meta`, `Packages`, `ProjectSettings`; ignore `Library`, `Temp`, `Logs`, `Obj`, `Builds` และ credentials

ตั้ง scene bootstrap, input modes เบื้องต้น, test assemblies, save location และ build entry point ที่เรียกซ้ำได้ คู่มือเปิดงานต้องบอก path/scene และวิธี build จริง ไม่ต้องทำ CI บนบริการภายนอกทันที

### M1 — ระบบที่เสี่ยงที่สุดต้องพิสูจน์ก่อน

ทำ shape model, local abrasion, rotation, end grinding, target overlay, evaluation, macro camera และ eye patch ใช้ asset ง่ายในห้องทดสอบ มี debug presets เฉพาะ Editor สำหรับชิ้นหนา/บาง/เสีย เพื่อทดสอบปลายทางโดยไม่ต้องเล่นหลายชั่วโมงทุกครั้ง

ต้องมีการเล่นจากเหล็กเต็มถึงเข็มอย่างน้อยหนึ่งครั้งโดยเส้นทางเดียวกับ build จริง ค่าเร่ง debug ใช้ช่วยตรวจ geometry ได้แต่ไม่นับเป็นหลักฐานบาลานซ์

ถ้าระบบฝนไม่ผ่าน อย่าไป M3–M5 เพียงเพราะฉากหรือร้านทำได้แล้ว จัดการข้อจำกัด representation ให้จบหรือแก้แบบอย่างเปิดเผย

### M2 — Vertical slice ที่ไม่หลอกด้วยเมนู

เชื่อมบ้าน graybox, NPC รับงาน, งานลูกหาบรอบเต็ม, wallet, คอม, 3 เครื่องมือแรก, delivery และเซฟ ผู้เล่นเดินไปทำกิจกรรมเอง ทุกทางเข้าสู่ระบบใช้ interaction เดียวกัน ไม่ต้องเปิด debug window

ทดสอบการเริ่มไม่มีเงินและการฝนพลาดจนต้องซื้อแท่งใหม่ตั้งแต่ขั้นนี้ ถ้าลูปยังไม่สนุก แก้ก่อนสร้างเครื่องมือสิบกว่าชิ้น

### M3 — แคมเปญครบโดยไม่ใช้บ่อน

เติมเครื่องมือทั้งหมดตาม data แล้วทำ special behavior เฉพาะตัวที่ต่างจริง เส้นทางงาน 3 แบบ รางวัลและ unlock, ร้านครบ, ตัวตรวจและจิ๊กเจาะ, ฉากจบและ collection ทำ balance spreadsheet/CSV ที่คำนวณจากค่าจริงของเกม ไม่ใช้ตัวเลขเอกสารคนละชุด

### M4 — บ่อนครบทั้งเจ็ดเกม

ทำ RNG/สำรับ/hand ranking/payout เป็น pure logic ก่อน UI ใช้ seed คงที่ใน test แต่ใช้ state ที่บันทึกได้ในการเล่นจริง เรียงงานจากเหรียญ/เต๋า/เป่ายิ้งฉุบ → สล็อต/High-Low → Blackjack → Poker → reload/settlement ของทุกเกม

ไม่อ้างว่าเกมเสร็จถ้าเหลือ Poker เป็น placeholder เพราะ user ขอชัดเจน สามารถสร้างเกมง่ายก่อนเพื่อใช้ตรวจ wallet แต่ทุกเกมต้องผ่านก่อน M4 ปิด

### M5 — เปลี่ยนจาก prototype เป็นผลิตภัณฑ์

ทำบ้าน ภูเขา บ่อน ตัวละครและอุปกรณ์ให้มี silhouette เดียวกัน ปรับแสง/วัสดุโดยตรวจกราฟิกต่ำด้วย เพิ่มเสียงและ feedback, settings, subtitle, font ไทย, tutorial ข้ามได้ และแม่แบบอีกสองแบบโดยไม่เพิ่มระบบใหม่

จัดทะเบียนสิทธิ์ asset/font/audio/code dependency พร้อมที่มาและเงื่อนไข เก็บบันทึกว่าเนื้อหาใดสร้างด้วย AI เพื่อกรอก Steam survey ตามจริงภายหลัง

### M6 — ทดสอบทั้งเกม

อย่างน้อย 5 ผู้ทดสอบที่ไม่เขียนระบบนี้ เล่น intro และเกมเต็มในกลุ่มที่กระจายทักษะเมาส์ เก็บผลแบบ local/แบบฟอร์ม ไม่ต้องเพิ่ม analytics online ผู้ใช้ตรวจท้ายได้ แต่ถ้ายังไม่มีคนทดสอบให้ระบุว่าเป็นงานค้างก่อนขาย

ตรวจว่าคนใหม่เข้าใจหมุนและ ghost โดยไม่บอก, อัปเกรดแรกเร็วพอ, การเสียชิ้นงานไม่รู้สึกสุ่ม, เล่น 20 นาทีแล้วเมื่อยหรือไม่ และบ่อนทำให้ loop หลักเสียสมดุลหรือไม่

เป้าประสิทธิภาพ: 60 FPS ที่ 1080p ระดับ medium บนเครื่องอ้างอิงที่จะบันทึกสเปกจริงใน M0/M6; บันทึก p95 frame time เป้าหมาย ≤16.7 ms, memory และช่วง autosave แยกจากค่าเฉลี่ย ยังไม่ประกาศ minimum spec จนทดสอบเครื่องที่อ้างถึงจริง

### M7 — ทำ Steam ให้ตรงกับ build

เพิ่ม Steam achievements/cloud ที่ประกาศไว้ โดยเกมพื้นฐานทำงาน offline เมื่อ Steam ไม่พร้อม ทดสอบ install ผ่าน depot จริงใน account ที่ได้รับสิทธิ์ ไม่ใช้แค่ exe ที่เปิดจาก Editor project

เตรียมภาพ gameplay จริง trailer สั้น capsule/icon คำบรรยายภาษาไทย/อังกฤษ credits และข้อมูล content สำหรับบ่อนจำลอง ไม่ใช้ concept image เป็น screenshot เกม

### M8 — ตรวจรับก่อนปล่อย

หยุดเพิ่ม feature แก้เฉพาะบั๊กของ RC ส่ง build ที่มี version/hash พร้อมรายการทดสอบให้ผู้ใช้ตรวจครบ ตรวจแพ็กเกจบนเครื่องสะอาดและเก็บ RC ก่อนหน้าเพื่อ rollback จากนั้นจึงขออนุมัติ publish ตาม build ที่ระบุ ไม่สัญญาว่าจะปล่อยขึ้น Steam อัตโนมัติเพราะแค่เขียนโค้ดจบ

## 7. การทดสอบที่ต้องมี

| ระบบ | Automated tests ที่มีประโยชน์ | ตรวจด้วยมือ |
|---|---|---|
| Grinding | local wear, nonnegative radius, no material gain, rotation invariance, seam, FPS replay, end clipping, exact/oversized/undercut target fixtures | รอยอ่านออก, ไม่ลากทะลุ, macro คุมได้, วัตถุไม่หายระหว่างซูม |
| Eye patch | hole inside head, seam continuity, dimensions preserved, reload same result | มองทะลุรูจริงและด้ายลอดตามตำแหน่ง |
| Wallet/shop | ไม่พอเงิน, negative input, overflow, double-click, claim twice, duplicate unique item | ปุ่ม/ข้อความ/ตำแหน่งกล่องเข้าใจได้ |
| Save | round trip shape, backup recovery, truncated file, future version, crash points around replace, migration fixtures | pause/quit/load ระหว่างกิจกรรมจริง ไม่มีของ/เงินหาย |
| Porter | payout boundaries, once-only completion, failure minimum, abort no reward, deterministic route validity | 55–75 วินาที, ไม่เมารถ, เล่น 10 รอบยังอ่านทางได้ |
| Poker | hand ranks ทุกระดับ, A-low straight, kickers, tie, no duplicate cards, draw bounds, turn order, pot conservation | NPC ไม่รู้ไพ่ผู้เล่น, ปุ่มไม่เปิดผิด turn |
| Blackjack | A หลายใบ, soft 17, naturals, push, bust, double, insufficient funds | dealer/ยอดเงินตรงกับกติกา UI |
| Other casino | Enumerate slot 1,000 ผล, dice 36 ผล, High-Low ทุก rank, RPS 9 ผล, payout rounding | Animation ตรงผลที่ commit, ไม่มีผลซ้ำจาก load |
| Progression | reward once, failed stock retains upgrades, ending requires every criterion | จบแบบไม่พนันและเล่นต่อหลังเครดิต |
| UI/localization | key coverage และ fallback ของข้อความสำคัญ | สระไทย, 720p/1080p/ultrawide, UI scale, keyboard navigation |

การทดสอบ RNG ทางสถิติเป็น supplementary ไม่ใช้ flaky test แบบสุ่มแล้วคาดว่าต้องได้เปอร์เซ็นต์เป๊ะ เมื่อ enumerate ได้ให้ตรวจผลทั้งหมดก่อน และแยก test ความยุติธรรมของ shuffle จากกติกาจ่ายเงิน

Smoke test หลังทุก milestone: New Game → ทำกิจกรรมที่เพิ่ม → Save → Quit → Load → ยืนยันสถานะ → Build Windows → เปิด executable ถ้ามีเพียง static review ให้ระบุไว้โดยตรง

## 8. Steam และงานที่ต้องใช้บัญชีเจ้าของ

การผ่าน checklist ทางเทคนิคไม่รับประกันว่า Valve จะอนุมัติหรือเกมจะขายดี ยังต้องมีคุณภาพ ประสบการณ์เล่น และ store presentation ที่ตรงกับของจริง

- Steam ต้องตรวจหน้า store และ product build; เอกสารระบุเวลาตรวจทั่วไป 3–5 วันทำการและให้เผื่ออย่างน้อย 7 วันทำการ จัดตารางรอและแก้ไขนอกเวลาพัฒนา [Steam review process](https://partner.steamgames.com/doc/store/review_process)
- Onboarding ต้องใช้ข้อมูลผู้เผยแพร่ ธนาคาร ภาษี และค่าผลิตภัณฑ์ เอกสารที่ตรวจวันจัดทำระบุค่าธรรมเนียม US$100 ต่อผลิตภัณฑ์, ระยะรอหลังจ่ายสำหรับเกมแรก ๆ 21 วัน และ Coming Soon อย่างน้อย 2 สัปดาห์ ตรวจข้อกำหนดใน account อีกครั้งก่อนกำหนดวันขายจริง [Steam onboarding](https://partner.steamgames.com/doc/gettingstarted/onboarding)
- กรอก Content Survey ให้ตรงกับบ่อนจำลองและเนื้อหาที่ส่ง รวมถึง AI-generated content ที่ผู้เล่นเห็น/ได้ยินตามคำถามปัจจุบัน ไม่สรุปว่าใช้ agent เขียนโค้ดอย่างเดียวเท่ากับต้องติดป้ายเนื้อหาทุกประเภท [Steam content survey](https://partner.steamgames.com/doc/gettingstarted/contentsurvey)
- รายละเอียด age rating และประเทศที่จำหน่ายต้องตรวจจาก survey/ข้อกำหนดปัจจุบันเมื่อมี build จริง ไม่สมมติว่าไม่มีเงินจริงแล้วขายได้ทุกประเทศโดยอัตโนมัติ
- การสมัครบัญชี กรอกข้อมูลส่วนตัว ชำระเงิน กำหนดราคา และกดเผยแพร่เป็นงานเจ้าของหรือการกระทำที่ต้องมีการอนุมัติชัดเจน แผนนี้ยังไม่ดำเนินการเหล่านั้น

ราคาเกมยังไม่ล็อก ต้องประเมินหลัง vertical slice/feedback และสำรวจตลาดใหม่ก่อนทำ store page; ไม่อ้างตัวเลขรายได้หรือยอดขายจากคอนเซ็ปต์เพียงอย่างเดียว

## 9. ความเสี่ยงและเกณฑ์หยุดขยายงาน

| ความเสี่ยง | สัญญาณเตือน | การรับมือ |
|---|---|---|
| Geometry หลักทำไม่ได้ | สึกทั้งวง, ลดความยาวไม่ได้, รูไม่ทะลุ | หยุดสร้าง content แล้วแก้ M1 ให้ผ่าน |
| สเกลช่วงท้ายไม่เป็นธรรม | ต้องเล็ง subpixel หรือผิดโดยไม่เห็น | macro/control mapping/tolerance ที่ทดสอบได้ |
| Grind น่าเบื่อ/เมื่อย | ผู้เล่นหยุดก่อนซื้อครั้งแรกหรือเกร็งมือ | ลดงานซ้ำ เพิ่ม feedback และเครื่องมือที่เปลี่ยนรูปแบบ |
| เริ่มใหม่เจ็บเกินไป | คนเลิกเล่นหลังพลาดครั้งเดียว | เตือนก่อนเสี่ยง, เหล็กใหม่ราคาต่ำ, เก็บเครื่องมือ, ประเมินเวลา retry |
| บ่อนกลบเกมหลัก | ลงทุนเครื่องมือจากสล็อตเร็วกว่าหาบเสมอ | ตรวจ RTP/เดิมพัน/เพดานรายได้ โดยไม่โกงผลตามผู้เล่น |
| เงินหรือของหาย | quit ระหว่าง delivery/settlement แล้วสถานะผิด | atomic snapshots และ crash/reload tests ก่อนเพิ่มเกม |
| Agent ทำ integration แตก | compile ผ่านเฉพาะไฟล์ตัวเอง, scene missing refs | เจ้าของไฟล์ชัด reviewer ทดสอบในโปรเจกต์จริง |
| Asset เสร็จช้ากว่า code | ยังมี placeholder ทุกฉากเมื่อถึง beta | งบ asset ชัด reuse modular และไม่เพิ่มพื้นที่ |
| Feature creep | เพิ่ม multiplayer/open world/ระบบหิว | เก็บ backlog หลัง 1.0 ไม่เปลี่ยน critical path |

## 10. Definition of Done

งานหนึ่งชิ้นเสร็จเมื่อ implementation, callers, data, UI และ test ที่กระทบครบ ผลรันจริงตรงเกณฑ์ ไม่มี error ใหม่ และ reviewer ตรวจแล้ว ไม่ใช่เมื่อ agent บอกว่าเขียนเสร็จ

Milestone เสร็จเมื่อ task ลูกผ่าน มี Windows build เปิดเล่นได้ และมีหลักฐานที่คนอื่นทำซ้ำได้

เกมพร้อมให้ผู้ใช้ตรวจท้ายเมื่อมี loop ครบ เจ็ดเกมบ่อนครบ เนื้อหาหลักไม่เป็น placeholder เซฟกู้คืนได้ และไม่มี blocker/critical bug ที่รู้แล้วค้างอยู่

เกมพร้อมเสนอขายเมื่อผู้ใช้ตรวจ RC ผ่าน, หลักฐานทดสอบเครื่องจริงครบตาม platform/features ที่ประกาศ, asset licenses/เนื้อหา/หน้า store ตรงจริง และ Steam review ผ่าน อย่าใช้คำว่า “พร้อมขาย” หากงานกลุ่มนี้ยังค้าง
