# 🚀 **즉시 실행 가이드**

## ⚡ **3분만에 게임 실행하기**

### 1️⃣ **Unity 설치** (5분)
```bash
# Unity Hub 다운로드
https://unity3d.com/get-unity/download

# Unity 2022.3 LTS 설치
- Unity Hub 열기
- "Installs" → "Install Editor" 
- 2022.3 LTS 선택
- Android Build Support 체크 ✅
```

### 2️⃣ **프로젝트 열기** (1분)
```bash
# Unity Hub에서
1. "Projects" → "Open"
2. flutter-game-app 폴더 선택
3. Unity 로딩 대기...
```

### 3️⃣ **자동 설정 실행** (30초)
```bash
# Unity 에디터 상단 메뉴에서
CozyMergeFarm → Full Auto Setup

# 또는 수동으로
CozyMergeFarm → Auto Setup Scene
CozyMergeFarm → Create Merge Item Prefab
```

### 4️⃣ **게임 실행!** (즉시) 
```bash
# Unity 에디터에서
1. ▶️ Play 버튼 클릭
2. Game 탭에서 플레이!
```

---

## 🎮 **게임 조작법**

### 🖱️ **마우스 (에디터)**
- **드래그**: 같은 식물끼리 드래그해서 합치기
- **클릭**: 광고 버튼 클릭

### 📱 **터치 (모바일)**  
- **드래그**: 같은 식물끼리 터치해서 합치기
- **탭**: 버튼들 터치

---

## 🌱 **게임플레이**

### ✨ **기본 룰**
1. **씨앗** 🌱 + **씨앗** 🌱 = **새싹** 🌿
2. **새싹** 🌿 + **새싹** 🌿 = **작은나무** 🌳
3. **작은나무** 🌳 + **작은나무** 🌳 = **큰나무** 🌲
4. **큰나무** 🌲 + **큰나무** 🌲 = **과일나무** 🍎

### 💰 **수익화**
- **머지할 때마다** → 코인 획득! 🪙
- **레벨이 높을수록** → 더 많은 코인!
- **광고 시청** → 보너스 코인! 🎁

### 🎯 **개발자 치트키** (PC만)
- **C키**: 코인 +1000개
- **L키**: 강제 레벨업  
- **T키**: 튜토리얼 리셋

---

## 🔧 **문제 해결**

### ❌ **스크립트 에러 발생 시**
```bash
# Unity 콘솔에 에러가 있다면
1. Edit → Project Settings → Player
2. Configuration → Scripting Backend: Mono
3. Api Compatibility Level: .NET Standard 2.1
```

### ❌ **DOTween 에러 발생 시** 
```bash
# Asset Store에서 DOTween 무료 다운로드
1. Window → Asset Store
2. "DOTween" 검색 → 무료 버전 다운로드
3. Import → Import All
```

### ❌ **UI가 안 보일 때**
```bash
# 다시 자동 설정 실행
CozyMergeFarm → Full Auto Setup
```

---

## 🎉 **성공했다면?**

게임이 정상 실행되면 다음을 볼 수 있어요:

- 🪙 **코인 카운터** (좌상단)
- 🌟 **레벨 표시** (우상단) 
- 🌻 **농장 이름** (상단 중앙)
- 🎁 **광고 버튼** (우하단)
- 🌱 **빈 그리드** (화면 중앙)

**자동으로 씨앗이 생성되고 드래그해서 합칠 수 있습니다!**

---

## 📱 **모바일 빌드**

### Android APK 생성
```bash
1. File → Build Settings
2. Platform: Android 선택
3. Switch Platform
4. Build → APK 생성
```

### iOS IPA 생성  
```bash
1. File → Build Settings
2. Platform: iOS 선택
3. Switch Platform  
4. Build → Xcode 프로젝트 생성
```

---

## 🆘 **도움이 필요하면**

- **GitHub Issues**: [flutter-game-app/issues](https://github.com/gyb0719/flutter-game-app/issues)
- **이메일**: gyb07190@gmail.com

---

## 🏆 **완료 체크리스트**

- [ ] Unity 2022.3 LTS 설치 완료
- [ ] 프로젝트 열기 성공
- [ ] 자동 설정 실행 완료  
- [ ] ▶️ Play 버튼으로 실행 성공
- [ ] 드래그 앤 드롭 테스트 성공
- [ ] 코인 획득 확인
- [ ] 광고 버튼 클릭 테스트

**모든 체크가 완료되면 게임이 완벽하게 동작합니다! 🎮✨**

---

**🌟 축하합니다! 코지 머지 농장이 성공적으로 실행되었습니다! 🌟**