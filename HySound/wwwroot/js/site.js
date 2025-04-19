document.addEventListener("DOMContentLoaded", function () {
    const isAlbumDetailsPage = window.location.pathname.includes('AlbumDetails');
    const isPlaylistDetailsPage = window.location.pathname.includes('PlaylistDetails');
    if (isAlbumDetailsPage || isPlaylistDetailsPage) return;

    const globalAudio = document.getElementById("global-audio");
    const globalAudioSource = document.getElementById("global-audio-source");
    const playButton = document.getElementById("play-btn");
    const prevButton = document.getElementById("prev-btn");
    const nextButton = document.getElementById("next-btn");
    const songImage = document.getElementById("song-image");
    const songTitle = document.getElementById("song-title");
    const artistName = document.getElementById("artist-name");
    const timebar = document.getElementById("timebar");
    const playerContainer = document.querySelector('.player-container');
    const soundBar = document.getElementById("sound-bar");

    const navLinks = document.getElementsByClassName("nav-link");

    let trackList = [];
    let currentTrackIndex = 0;
    let isPlaying = false;
    let selectedAlbumId = null;
    let currentTracks = [];
    let selectedPlaylistId = null;

    function updateProgressBar(bar, value) {
        bar.style.background = `linear-gradient(to right, #1db954 0%, #1db954 ${value}%, #1e1e1e ${value}%, #1e1e1e 100%)`;
    }

    function loadStoredTrack() {
        const storedTrack = localStorage.getItem("currentTrack");
        if (storedTrack) {
            const { file, title, artist, image, time, isCurrentlyPlayed } = JSON.parse(storedTrack);
            globalAudioSource.src = file;
            globalAudio.load();
            globalAudio.currentTime = time || 0;
            songImage.src = image;
            songTitle.textContent = title;
            artistName.textContent = artist;
            timebar.value = (time / globalAudio.duration) * 100 || 0;
            updateProgressBar(timebar, timebar.value);
            globalAudio.volume = 1.0;
            soundBar.value = globalAudio.volume * 100;
            updateProgressBar(soundBar, soundBar.value);

            if (isCurrentlyPlayed) {
                globalAudio.play();
                playButton.querySelector("i").classList.replace("fa-play", "fa-pause");
                isPlaying = true;
                playerContainer.classList.add('visible');
            }
        } else {
            loadTrack(0, false);
        }
    }

    for (let navLink of navLinks) {
        navLink.addEventListener("click", saveTrackState);
    }

    playButton.addEventListener("click", function () {
        if (globalAudio.paused) {
            globalAudio.play();
            playButton.querySelector("i").classList.replace("fa-play", "fa-pause");
            isPlaying = true;
            playerContainer.classList.add('visible');
        } else {
            globalAudio.pause();
            playButton.querySelector("i").classList.replace("fa-pause", "fa-play");
            isPlaying = false;
        }
        saveTrackState();
    });

    prevButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex - 1 + trackList.length) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

    nextButton.addEventListener("click", function () {
        currentTrackIndex = (currentTrackIndex + 1) % trackList.length;
        loadTrack(currentTrackIndex, isPlaying);
    });

    function loadTrack(index, shouldPlay = false) {
        const track = trackList[index];
        console.log('Зареждане на песен:', track);

        globalAudioSource.src = track.file;
        songImage.src = track.image || 'path/to/default/image.jpg';
        console.log('Изображението е зададено на:', songImage.src);

        songTitle.textContent = track.title || 'Песен без име';
        console.log('Заглавието е зададено на:', songTitle.textContent);

        artistName.textContent = track.artist || 'Неизвестен изпълнител';
        console.log('Изпълнителят е зададен на:', artistName.textContent);

        globalAudio.load();
        globalAudio.currentTime = 0;
        timebar.value = 0;
        updateProgressBar(timebar, 0);
        globalAudio.volume = 1.0;
        soundBar.value = 100;
        updateProgressBar(soundBar, 100);

        if (shouldPlay) {
            globalAudio.addEventListener('canplay', function playTrack() {
                globalAudio.play();
                playButton.querySelector("i").classList.replace("fa-play", "fa-pause");
                isPlaying = true;
                playerContainer.classList.add('visible');
                globalAudio.removeEventListener('canplay', playTrack);
            });
        } else {
            playButton.querySelector("i").classList.replace("fa-pause", "fa-play");
            isPlaying = false;
            playerContainer.classList.remove('visible');
        }
        saveTrackState();
    }

    function saveTrackState() {
        localStorage.setItem("currentTrack", JSON.stringify({
            file: globalAudioSource.src,
            title: songTitle.textContent,
            artist: artistName.textContent,
            image: songImage.src,
            time: globalAudio.currentTime,
            isCurrentlyPlayed: isPlaying
        }));
    }

    globalAudio.addEventListener("timeupdate", function () {
        if (!isNaN(globalAudio.duration)) {
            const progress = (globalAudio.currentTime / globalAudio.duration) * 100;
            timebar.value = progress || 0;
            updateProgressBar(timebar, timebar.value);
        }
    });

    timebar.addEventListener("input", function () {
        globalAudio.currentTime = (timebar.value / 100) * globalAudio.duration;
        updateProgressBar(timebar, timebar.value);
        saveTrackState();
    });

    soundBar.addEventListener("input", function () {
        globalAudio.volume = soundBar.value / 100;
        updateProgressBar(soundBar, soundBar.value);
    });

    trackList = [
        {
            file: 'path-to-song1.mp3',
            title: 'Песен 1',
            artist: 'Изпълнител 1',
            image: 'path-to-image1.jpg'
        },
        {
            file: 'path-to-song2.mp3',
            title: 'Песен 2',
            artist: 'Изпълнител 2',
            image: 'path-to-image2.jpg'
        },
        {
            file: 'path-to-song3.mp3',
            title: 'Песен 3',
            artist: 'Изпълнител 3',
            image: 'path-to-image3.jpg'
        }
    ];

    loadStoredTrack();

    document.getElementById('favourites-option').addEventListener('click', function () {
        toggleContent('favourites');
        updateTitle('Любими');
        updateActiveState('favourites');
    });

    document.getElementById('playlists-option').addEventListener('click', function () {
        toggleContent('playlists');
        updateTitle('Плейлисти');
        updateActiveState('playlists');
    });

    document.getElementById('albums-option').addEventListener('click', function () {
        toggleContent('albums');
        updateTitle('Албуми');
        updateActiveState('albums');
    });

    function toggleContent(content) {
        document.getElementById('favourites-content').style.display = 'none';
        document.getElementById('playlists-content').style.display = 'none';
        document.getElementById('albums-content').style.display = 'none';
        document.getElementById('selected-album-tracks').style.display = 'none';
        document.getElementById('selected-playlist-tracks').style.display = 'none';

        if (content === 'favourites') {
            document.getElementById('favourites-content').style.display = 'grid';
        } else if (content === 'playlists') {
            if (selectedPlaylistId) {
                showPlaylistTracks(selectedPlaylistId);
            } else {
                document.getElementById('playlists-content').style.display = 'grid';
            }
        } else if (content === 'albums') {
            if (selectedAlbumId) {
                showAlbumTracks(selectedAlbumId);
            } else {
                document.getElementById('albums-content').style.display = 'grid';
            }
        }
    }

    function updateTitle(title) {
        document.getElementById('library-title').textContent = title;
    }

    function updateActiveState(option) {
        const navItems = document.querySelectorAll('.library-nav li');
        navItems.forEach(item => item.classList.remove('active'));
        document.getElementById(option + '-option').classList.add('active');
    }

    document.addEventListener("click", function (e) {
        const albumCard = e.target.closest('.album-card');
        if (albumCard && albumCard.dataset.type === 'album') {
            const playButton = albumCard.querySelector('.album-play-btn');
            if (e.target === playButton || playButton.contains(e.target)) {
                const albumId = playButton.dataset.albumid;
                if (albumId) {
                    console.log('Възпроизвеждане на албум с ID:', albumId);
                    fetchAlbumTracks(albumId).then(tracks => {
                        trackList = tracks.map(track => ({
                            file: track.audioUrl || track.file || '',
                            title: track.name || track.title || 'Песен без име',
                            artist: track.userName || track.artist || track.creator || 'Неизвестен изпълнител',
                            image: track.imageLink || track.image || track.coverImage || ''
                        })).filter(track => track.file && track.file.trim().length > 0);
                        console.log('Създаден списък с песни за албума:', trackList);
                        if (trackList.length > 0) {
                            currentTrackIndex = 0;
                            loadTrack(0, true);
                        } else {
                            console.log('Няма валидни песни в този албум');
                        }
                    }).catch(error => console.error('Грешка при извличане:', error));
                }
            } else {
                const albumId = albumCard.dataset.albumid;
                if (albumId) {
                    console.log('Показване на мрежа за албум с ID:', albumId);
                    showAlbumTracks(albumId);
                }
            }
        }

        const playlistCard = e.target.closest('.library-track-card[data-type="playlist"]');
        if (playlistCard) {
            const playButton = playlistCard.querySelector('.playlist-play-btn');
            if (e.target === playButton || playButton.contains(e.target)) {
                const playlistId = playButton.dataset.playlistid;
                if (playlistId) {
                    console.log('Възпроизвеждане на плейлист с ID:', playlistId);
                    fetchPlaylistTracks(playlistId).then(tracks => {
                        trackList = tracks.map(track => ({
                            file: track.audioUrl || track.file || '',
                            title: track.name || track.title || 'Песен без име',
                            artist: track.userName || track.artist || track.creator || 'Неизвестен изпълнител',
                            image: track.imageLink || track.image || track.coverImage || ''
                        })).filter(track => track.file && track.file.trim().length > 0);
                        console.log('Създаден списък с песни за плейлиста:', trackList);
                        if (trackList.length > 0) {
                            currentTrackIndex = 0;
                            loadTrack(0, true);
                        } else {
                            console.log('Няма валидни песни в този плейлист');
                        }
                    }).catch(error => console.error('Грешка при извличане:', error));
                }
            } else {
                const playlistId = playlistCard.dataset.playlistid;
                if (playlistId) {
                    console.log('Показване на мрежа за плейлист с ID:', playlistId);
                    showPlaylistTracks(playlistId);
                }
            }
        }

        const trackPlayButton = e.target.closest('.tracklist-play-btn');
        if (trackPlayButton) {
            const index = parseInt(trackPlayButton.dataset.index);
            if (!isNaN(index) && currentTracks && currentTracks.length > index) {
                trackList = currentTracks.slice(index).map(track => ({
                    file: track.audioUrl || track.file || '',
                    title: track.name || track.title || 'Песен без име',
                    artist: track.userName || track.artist || track.creator || 'Неизвестен изпълнител',
                    image: track.imageLink || track.image || track.coverImage || ''
                })).filter(track => track.file && track.file.trim().length > 0);
                console.log('Създаден списък с песни от мрежата:', trackList);
                if (trackList.length > 0) {
                    currentTrackIndex = 0;
                    loadTrack(0, true);
                } else {
                    console.log('Няма валидни песни от тази точка');
                }
            } else {
                const file = trackPlayButton.dataset.src;
                const title = trackPlayButton.dataset.title || 'Песен без име';
                const artist = trackPlayButton.dataset.artist || 'Неизвестен изпълнител';
                const image = trackPlayButton.dataset.image || '';
                if (file && file !== 'undefined') {
                    trackList = [{ file, title, artist, image }];
                    currentTrackIndex = 0;
                    loadTrack(0, true);
                } else {
                    console.log('Няма валиден URL адрес за аудио за тази песен в мрежата:', file);
                }
            }
        }

        const singlePlayButton = e.target.closest('.play-track-btn, .track-play-btn-home');
        if (singlePlayButton) {
            console.log('Кликнат е бутон за единична песен:', singlePlayButton);
            const file = singlePlayButton.dataset.src;
            const title = singlePlayButton.dataset.title || 'Песен без име';
            const artist = singlePlayButton.dataset.artist || 'Неизвестен изпълнител';
            const image = singlePlayButton.dataset.image || '';
            if (file && file !== 'undefined') {
                trackList = [{ file, title, artist, image }];
                currentTrackIndex = 0;
                loadTrack(0, true);
            } else {
                console.log('Няма валиден URL адрес за аудио за тази единична песен:', file);
            }
        }
    });

    async function fetchAlbumTracks(albumId) {
        try {
            const response = await fetch('/Album/Tracks?albumId=' + albumId);
            if (!response.ok) {
                throw new Error('Неуспешно извличане на песни от албума: Сървърът върна ' + response.status);
            }
            const tracks = await response.json();
            console.log('Необработени данни за песни от албума:', tracks);
            return tracks;
        } catch (error) {
            console.error(error);
            alert('Грешка при извличане на песни от албума. Моля, опитайте отново по-късно.');
            return [];
        }
    }

    async function showAlbumTracks(albumId) {
        selectedAlbumId = albumId;
        hideAlbumTracksGrid();
        const tracks = await fetchAlbumTracks(albumId);
        currentTracks = tracks;
        console.log('Песни за показване в мрежата:', tracks);
        populateTracksGrid(tracks);
        document.getElementById('albums-content').style.display = 'none';
        document.getElementById('selected-album-tracks').style.display = 'grid';

        const backButton = document.getElementById('back-to-albums');
        if (!backButton) {
            const backButtonHtml = `<button id="back-to-albums" class="back-button">Обратно към албумите</button>`;
            document.getElementById('selected-album-tracks').insertAdjacentHTML('beforeend', backButtonHtml);
            document.getElementById('back-to-albums').addEventListener('click', hideAlbumTracksGrid);
        }
    }

    function hideAlbumTracksGrid() {
        selectedAlbumId = null;
        document.getElementById('albums-content').style.display = 'grid';
        document.getElementById('selected-album-tracks').style.display = 'none';
        const backButton = document.getElementById('back-to-albums');
        if (backButton) {
            backButton.remove();
        }
    }

    async function fetchPlaylistTracks(playlistId) {
        try {
            const response = await fetch('/Playlist/Tracks?playlistId=' + playlistId);
            if (!response.ok) {
                throw new Error('Неуспешно извличане на песни от плейлиста: Сървърът върна ' + response.status);
            }
            const tracks = await response.json();
            console.log('Необработени данни за песни от плейлиста:', tracks);
            return tracks;
        } catch (error) {
            console.error(error);
            alert('Грешка при извличане на песни от плейлиста. Моля, опитайте отново по-късно.');
            return [];
        }
    }

    async function showPlaylistTracks(playlistId) {
        selectedPlaylistId = playlistId;
        hidePlaylistTracksGrid();
        const tracks = await fetchPlaylistTracks(playlistId);
        currentTracks = tracks;
        console.log('Песни за показване в мрежата на плейлиста:', tracks);
        populateTracksGrid(tracks, 'selected-playlist-tracks');
        document.getElementById('playlists-content').style.display = 'none';
        document.getElementById('selected-playlist-tracks').style.display = 'grid';

        const backButton = document.getElementById('back-to-playlists');
        if (!backButton) {
            const backButtonHtml = `<button id="back-to-playlists" class="back-button">Обратно към плейлистите</button>`;
            document.getElementById('selected-playlist-tracks').insertAdjacentHTML('beforeend', backButtonHtml);
            document.getElementById('back-to-playlists').addEventListener('click', hidePlaylistTracksGrid);
        }
    }

    function hidePlaylistTracksGrid() {
        selectedPlaylistId = null;
        document.getElementById('playlists-content').style.display = 'grid';
        document.getElementById('selected-playlist-tracks').style.display = 'none';
        const backButton = document.getElementById('back-to-playlists');
        if (backButton) {
            backButton.remove();
        }
    }

    function populateTracksGrid(tracks, containerId = 'selected-album-tracks') {
        const grid = document.getElementById(containerId);
        grid.innerHTML = '';
        grid.className = 'tracklist-grid';
        if (!tracks || tracks.length === 0) {
            grid.innerHTML = '<p class="no-tracks">Няма песни в този плейлист.</p>';
        } else {
            tracks.forEach((track, index) => {
                const rowHtml = `
                    <div class="tracklist-row">
                        <div class="tracklist-cell image">
                            <img src="${track.imageLink || track.image || track.coverImage || ''}" alt="Изображение на песен" class="tracklist-image">
                        </div>
                        <div class="tracklist-cell title">${track.name || track.title || 'Песен без име'}</div>
                        <div class="tracklist-cell artist">${track.userName || track.artist || track.creator || 'Неизвестен изпълнител'}</div>
                        <div class="tracklist-cell actions">
                            <button class="tracklist-play-btn" 
                                    data-index="${index}"
                                    data-src="${track.audioUrl || track.file || ''}" 
                                    data-title="${track.name || track.title || ''}" 
                                    data-artist="${track.userName || track.artist || track.creator || ''}" 
                                    data-image="${track.imageLink || track.image || track.coverImage || ''}">
                                <i class="fas fa-play"></i>
                            </button>
                        </div>
                    </div>`;
                grid.innerHTML += rowHtml;
            });
        }
    }
});

$(document).ready(function () {
    fetchNotifications();
});

function fetchNotifications() {
    $.get("/api/notifications/unread", function (data) {
        let notificationList = $("#notifications");
        notificationList.empty();

        data.forEach(notification => {
            let listItem = `<li>${notification.message} <button onclick="markAsRead(${notification.id})">Отбележи като прочетено</button></li>`;
            notificationList.append(listItem);
        });
    });
}

function markAsRead(id) {
    $.post(`/api/notifications/mark-read/${id}`, function () {
        fetchNotifications();
    });
}

document.addEventListener('DOMContentLoaded', function () {
    const player = document.querySelector('.player-container');
    const modals = document.querySelectorAll('.modal');

    function togglePlayerVisibility() {
        modals.forEach(modal => {
            if (modal.style.display === 'flex' || modal.style.display === 'block') {
                player.classList.remove('visible');
            } else if (!Array.from(modals).some(m => m.style.display === 'flex' || m.style.display === 'block')) {
                player.classList.add('visible');
            }
        });
    }

    modals.forEach(modal => {
        modal.addEventListener('click', togglePlayerVisibility);
    });

    const openEditModal = () => {
        document.getElementById('editModal').style.display = 'flex';
        player.classList.remove('visible');
    };
    const closeEditModal = () => {
        document.getElementById('editModal').style.display = 'none';
        player.classList.add('visible');
    };

    document.querySelector('.edit-profile-btn')?.addEventListener('click', openEditModal);
    document.querySelector('.close-btn')?.addEventListener('click', closeEditModal);
});

document.addEventListener('DOMContentLoaded', function () {
    const player = document.querySelector('.player-container');
    const body = document.body;

    const playerHeight = player.offsetHeight || 70;
    body.style.paddingBottom = `${playerHeight}px`;

    player.classList.add('visible');
});